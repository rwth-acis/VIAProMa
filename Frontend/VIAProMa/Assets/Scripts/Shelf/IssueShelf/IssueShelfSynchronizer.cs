using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class IssueShelfSynchronizer : TransformSynchronizer
{
    [SerializeField] private ShelfConfigurationMenu configurationMenu;

    private IssuesLoader issueLoader;

    private int remoteSynchronizations = 0;
    private bool initialized = false;

    private bool RemoteSynchronizationInProgress { get => remoteSynchronizations > 0; }

    private void Awake()
    {
        if (configurationMenu == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(configurationMenu));
        }
        issueLoader = GetComponent<IssuesLoader>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            initialized = true;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (!RemoteSynchronizationInProgress && initialized)
        {
            photonView.RPC("SetActive", RpcTarget.Others, true);
        }
        configurationMenu.SourceChanged += OnSourceChanged;
        configurationMenu.ReqBazProjectChanged += OnReqBazProjectChanged;
        configurationMenu.ReqBazCategoryChanged += OnReqBazCategoryChanged;
        configurationMenu.GitHubOwnerChanged += OnGitHubOwnerChanged;
        configurationMenu.GitHubProjectChanged += OnGitHubProjectChanged;
        configurationMenu.WindowOpened += OnConfigWindowOpened;
        configurationMenu.WindowClosed += OnConfigWindowClosed;
        issueLoader.PageChanged += OnPageChanged;
        issueLoader.SearchFieldChanged += OnSearchFieldChanged;
    }

    public override void OnDisable()
    {
        if (!RemoteSynchronizationInProgress)
        {
            photonView.RPC("SetActive", RpcTarget.Others, false);
        }
        configurationMenu.SourceChanged -= OnSourceChanged;
        configurationMenu.ReqBazProjectChanged -= OnReqBazProjectChanged;
        configurationMenu.ReqBazCategoryChanged -= OnReqBazCategoryChanged;
        configurationMenu.GitHubOwnerChanged -= OnGitHubOwnerChanged;
        configurationMenu.GitHubProjectChanged -= OnGitHubProjectChanged;
        configurationMenu.WindowOpened -= OnConfigWindowOpened;
        configurationMenu.WindowClosed -= OnConfigWindowClosed;
        issueLoader.PageChanged -= OnPageChanged;
        issueLoader.SearchFieldChanged -= OnSearchFieldChanged;
        base.OnDisable();
    }

    public override async void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && newPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
        {
            short gitHubOwnerStringId = await NetworkedStringManager.StringToId(configurationMenu.GitHubOwner);
            short gitHubProjectStringId = await NetworkedStringManager.StringToId(configurationMenu.GitHubRepository);
            short searchStringId = await NetworkedStringManager.StringToId(issueLoader.SearchFilter);

            // the master client informs the new player about the current status
            if (configurationMenu.ShelfConfiguration.SelectedSource == DataSource.REQUIREMENTS_BAZAAR)
            {
                ReqBazShelfConfiguration config = (ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration;
                short selectedProjectId;
                if (config.SelectedProject == null)
                {
                    selectedProjectId = -1;
                }
                else
                {
                    selectedProjectId = (short)config.SelectedProject.id;
                }
                short selectedCategoryId;
                if (config.SelectedCategory == null)
                {
                    selectedCategoryId = -1;
                }
                else
                {
                    selectedCategoryId = (short)config.SelectedCategory.id;
                }

                photonView.RPC("Initialize", RpcTarget.Others,
                gameObject.activeSelf,
                configurationMenu.WindowOpen,
                (byte)config.SelectedSource,
                selectedProjectId,
                selectedCategoryId,
                gitHubOwnerStringId,
                gitHubProjectStringId,
                (short)issueLoader.Page,
                searchStringId
                );
            }
            else
            {
                photonView.RPC("Initialize", RpcTarget.Others,
                gameObject.activeSelf,
                configurationMenu.WindowOpen,
                (byte)configurationMenu.ShelfConfiguration.SelectedSource,
                (short)-1,
                (short)-1,
                gitHubOwnerStringId,
                gitHubProjectStringId,
                (short)issueLoader.Page,
                searchStringId
                );
            }
        }
    }

    [PunRPC]
    private async void Initialize(
        bool activeState,
        bool configWindowOpen,
        byte sourceId,
        short reqBazProjectId,
        short reqBazCategoryId,
        short gitHubOwnerStringId,
        short gitHubProjectStringId,
        short page,
        short searchStringId
        )
    {
        Debug.Log("RPC: initialize the issue shelf");

        remoteSynchronizations++;

        configurationMenu.ExternallyInitialized = true;
        await configurationMenu.Initialize();

        // initializes the configuration
        SetActive(activeState);
        SetConfigWindow(configWindowOpen);
        if (reqBazProjectId != -1)
        {
            await SetReqBazProject(reqBazProjectId);
        }
        if (reqBazCategoryId != -1)
        {
            SetReqBazCategory(reqBazCategoryId);
        }
        await SetGitHubOwner(gitHubOwnerStringId);
        await SetGitHubProject(gitHubProjectStringId);
        SetPage(page);
        SetSearchField(searchStringId);

        await SetSource(sourceId); // source must be set last because the individual settings for ReqBaz and GitHub also change the source

        remoteSynchronizations--;
        initialized = true;
        Debug.Log("Client is now initialized.");
    }

    [PunRPC]
    private async void SetStringIds(short gitHubOwnerStringId, short gitHubProjectStringId)
    {
        remoteSynchronizations++;
        Debug.Log("RPC: set string ids to " + gitHubOwnerStringId + " and " + gitHubProjectStringId, gameObject);
        await SetGitHubOwner(gitHubOwnerStringId);
        await SetGitHubProject(gitHubProjectStringId);
        remoteSynchronizations--;
    }

    [PunRPC]
    private void SetActive(bool active)
    {
        remoteSynchronizations++;
        Debug.Log("RPC: set issue shelf active to " + active, gameObject);
        gameObject.SetActive(active);
        remoteSynchronizations--;
    }

    [PunRPC]
    private async Task SetSource(byte sourceId)
    {
        remoteSynchronizations++;
        Debug.Log("RPC: set data source to " + sourceId, gameObject);
        await configurationMenu.SetDataSource((DataSource)sourceId);
        remoteSynchronizations--;
    }

    [PunRPC]
    private async Task SetReqBazProject(short projectId)
    {
        remoteSynchronizations++;
        if (configurationMenu.ShelfConfiguration.SelectedSource == DataSource.REQUIREMENTS_BAZAAR)
        {
            await configurationMenu.SetReqBazProject(projectId);
        }
        else
        {
            Debug.LogError("RPC tried to change Requirements Bazaar project but Requirements Bazaar is not selected as source.", gameObject);
        }
        remoteSynchronizations--;
    }

    [PunRPC]
    private void SetReqBazCategory(short categoryId)
    {
        remoteSynchronizations++;
        if (configurationMenu.ShelfConfiguration.SelectedSource == DataSource.REQUIREMENTS_BAZAAR)
        {
            configurationMenu.SetReqBazCategory(categoryId);
        }
        else
        {
            Debug.LogError("RPC tried to change Requirements Bazaar project but Requirements Bazaar is not selected as source.", gameObject);
        }
        remoteSynchronizations--;
    }

    [PunRPC]
    private async Task SetGitHubOwner(short gitHubOwnerStringId)
    {
        remoteSynchronizations++;
        string gitHubOwner = await NetworkedStringManager.GetString(gitHubOwnerStringId);
        Debug.Log("RPC: set GitHubOwner to " + gitHubOwner, gameObject);
        configurationMenu.SetGitHubOwner(gitHubOwner);
        remoteSynchronizations--;
    }

    [PunRPC]
    private async Task SetGitHubProject(short gitHubProjectStringId)
    {
        remoteSynchronizations++;
        string gitHubProject = await NetworkedStringManager.GetString(gitHubProjectStringId);
        Debug.Log("RPC: set GitHubProject to " + gitHubProject, gameObject);
        configurationMenu.SetGitHubProject(gitHubProject);
        remoteSynchronizations--;
    }

    [PunRPC]
    private void SetConfigWindow(bool open)
    {
        remoteSynchronizations++;
        Debug.Log("RPC: set Configuration Window open to " + open);
        if (open)
        {
            configurationMenu.Open();
        }
        else
        {
            configurationMenu.Close();
        }
        remoteSynchronizations--;
    }

    [PunRPC]
    private void SetPage(short page)
    {
        remoteSynchronizations++;
        Debug.Log("RPC: set page to " + page);
        issueLoader.Page = page;
        remoteSynchronizations--;
    }

    [PunRPC]
    private async void SetSearchField(short searchStringId)
    {
        remoteSynchronizations++;
        string searchString = await NetworkedStringManager.GetString(searchStringId);
        Debug.Log("RPC: set search field to " + searchString);
        issueLoader.SearchFilter = searchString;
        remoteSynchronizations--;
    }

    private void OnSourceChanged(object sender, EventArgs e)
    {
        if (RemoteSynchronizationInProgress || !initialized)
        {
            return;
        }
        photonView.RPC("SetSource", RpcTarget.Others, (byte)configurationMenu.ShelfConfiguration.SelectedSource);
    }

    private void OnReqBazProjectChanged(object sender, EventArgs e)
    {
        Debug.Log("REQ Baz Project changed; remotesync in progress: " + remoteSynchronizations);
        if (RemoteSynchronizationInProgress || !initialized)
        {
            return;
        }

        if (configurationMenu.ShelfConfiguration.SelectedSource == DataSource.REQUIREMENTS_BAZAAR)
        {
            short projectId = (short)((ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration).SelectedProject.id;
            photonView.RPC("SetReqBazProject", RpcTarget.Others, projectId);
        }
        else
        {
            Debug.LogError("Tried to send RPC for changed Requirements Bazaar project but Requirements Bazaar is not selected as data source");
        }
    }

    private void OnReqBazCategoryChanged(object sender, EventArgs e)
    {
        if (RemoteSynchronizationInProgress || !initialized)
        {
            return;
        }

        if (configurationMenu.ShelfConfiguration.SelectedSource == DataSource.REQUIREMENTS_BAZAAR)
        {
            short categoryId;
            Category selectedCategory = ((ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration).SelectedCategory;
            if (selectedCategory == null)
            {
                categoryId = -1;
            }
            else
            {
                categoryId = (short)((ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration).SelectedCategory.id;
            }
            photonView.RPC("SetReqBazCategory", RpcTarget.Others, categoryId);
        }
        else
        {
            Debug.LogError("Tried to send RPC for changed Requirements Bazaar category but Requirements Bazaar is not selected as data source");
        }
    }

    private async void OnGitHubOwnerChanged(object sender, EventArgs e)
    {
        if (RemoteSynchronizationInProgress || !initialized)
        {
            return;
        }
        short gitHubOwnerStringId = await NetworkedStringManager.StringToId(configurationMenu.GitHubOwner);
        photonView.RPC("SetGitHubOwner", RpcTarget.Others, gitHubOwnerStringId);
    }

    private async void OnGitHubProjectChanged(object sender, EventArgs e)
    {
        if (RemoteSynchronizationInProgress || !initialized)
        {
            return;
        }

        short gitHubProjectStringId = await NetworkedStringManager.StringToId(configurationMenu.GitHubRepository);
        photonView.RPC("SetGitHubProject", RpcTarget.Others, gitHubProjectStringId);
    }

    private void OnConfigWindowOpened(object sender, EventArgs e)
    {
        if (RemoteSynchronizationInProgress || !initialized)
        {
            return;
        }
        photonView.RPC("SetConfigWindow", RpcTarget.Others, true);
    }

    private void OnConfigWindowClosed(object sender, EventArgs e)
    {
        if (RemoteSynchronizationInProgress || !initialized)
        {
            return;
        }
        photonView.RPC("SetConfigWindow", RpcTarget.Others, false);
    }

    private void OnPageChanged(object sender, EventArgs e)
    {
        if (RemoteSynchronizationInProgress || !initialized)
        {
            return;
        }
        photonView.RPC("SetPage", RpcTarget.Others, (short)issueLoader.Page);
    }

    private async void OnSearchFieldChanged(object sender, EventArgs e)
    {
        if (RemoteSynchronizationInProgress || !initialized)
        {
            return;
        }
        short searchStringId = await NetworkedStringManager.StringToId(issueLoader.SearchFilter);
        photonView.RPC("SetSearchField", RpcTarget.Others, searchStringId);
    }
}
