using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssueShelfSynchronizer : TransformSynchronizer
{
    [SerializeField] private ShelfConfigurationMenu configurationMenu;

    private IssuesLoader issueLoader;

    private void Awake()
    {
        if (configurationMenu == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(configurationMenu));
        }
        issueLoader = GetComponent<IssuesLoader>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        photonView.RPC("SetActive", RpcTarget.Others, true);
        configurationMenu.SourceChanged += OnSourceChanged;
        configurationMenu.ReqBazProjectChanged += OnReqBazProjectChanged;
        configurationMenu.ReqBazCategoryChanged += OnReqBazCategoryChanged;
        configurationMenu.GitHubOwnerChanged += OnGitHubOwnerChanged;
        configurationMenu.GitHubProjectChanged += OnGitHubProjectChanged;
        configurationMenu.WindowOpened += OnConfigWindowOpened;
        configurationMenu.WindowClosed += OnConfigWindowClosed;
        issueLoader.PageChanged += OnPageChanged;
    }

    public override void OnDisable()
    {
        photonView.RPC("SetActive", RpcTarget.Others, false);
        configurationMenu.SourceChanged -= OnSourceChanged;
        configurationMenu.ReqBazProjectChanged -= OnReqBazProjectChanged;
        configurationMenu.ReqBazCategoryChanged -= OnReqBazCategoryChanged;
        configurationMenu.GitHubOwnerChanged -= OnGitHubOwnerChanged;
        configurationMenu.GitHubProjectChanged -= OnGitHubProjectChanged;
        configurationMenu.WindowOpened -= OnConfigWindowOpened;
        configurationMenu.WindowClosed -= OnConfigWindowClosed;
        issueLoader.PageChanged -= OnPageChanged;
        base.OnDisable();
    }

    public override async void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && newPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
        {
            short gitHubOwnerStringId = await NetworkedStringManager.StringToId(configurationMenu.GitHubOwner);
            short gitHubProjectStringId = await NetworkedStringManager.StringToId(configurationMenu.GitHubRepository);

            // the master client informs the new player about the current status
            if (configurationMenu.ShelfConfiguration.SelectedSource == DataSource.REQUIREMENTS_BAZAAR)
            {
                ReqBazShelfConfiguration config = (ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration;
                photonView.RPC("Initialize", RpcTarget.Others,
                gameObject.activeSelf,
                configurationMenu.WindowOpen,
                (byte)config.SelectedSource,
                (short)config.SelectedProject.id,
                (short)config.SelectedCategory.id,
                gitHubOwnerStringId,
                gitHubProjectStringId,
                (short)issueLoader.Page
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
                (short)issueLoader.Page
                );
            }
        }
    }

    [PunRPC]
    private void Initialize(
        bool activeState,
        bool configWindowOpen,
        byte sourceId,
        short reqBazProjectId,
        short reqBazCategoryId,
        short gitHubOwnerStringId,
        short gitHubProjectStringId,
        short page
        )
    {
        // initializes the configuration
        SetActive(activeState);
        SetConfigWindow(configWindowOpen);
        SetSource(sourceId);
        if (reqBazProjectId != -1)
        {
            SetReqBazProject(reqBazProjectId);
        }
        if (reqBazCategoryId != -1)
        {
            SetReqBazCategory(reqBazCategoryId);
        }
        SetGitHubOwner(gitHubOwnerStringId);
        SetGitHubProject(gitHubProjectStringId);
        SetPage(page);
    }

    [PunRPC]
    private void SetStringIds(short gitHubOwnerStringId, short gitHubProjectStringId)
    {
        Debug.Log("RPC: set string ids to " + gitHubOwnerStringId + " and " + gitHubProjectStringId, gameObject);
        SetGitHubOwner(gitHubOwnerStringId);
        SetGitHubProject(gitHubProjectStringId);
    }

    [PunRPC]
    private void SetActive(bool active)
    {
        Debug.Log("RPC: set issue shelf active to " + active, gameObject);
        gameObject.SetActive(active);
    }

    [PunRPC]
    private void SetSource(byte sourceId)
    {
        Debug.Log("RPC: set data source to " + sourceId, gameObject);
        configurationMenu.SetDataSource((DataSource)sourceId);
    }

    [PunRPC]
    private void SetReqBazProject(short projectId)
    {
        if (configurationMenu.ShelfConfiguration.SelectedSource == DataSource.REQUIREMENTS_BAZAAR)
        {
            configurationMenu.SetReqBazProject(projectId);
        }
        else
        {
            Debug.LogError("RPC tried to change Requirements Bazaar project but Requirements Bazaar is not selected as source.", gameObject);
        }
    }

    [PunRPC]
    private void SetReqBazCategory(short categoryId)
    {
        if (configurationMenu.ShelfConfiguration.SelectedSource == DataSource.REQUIREMENTS_BAZAAR)
        {
            configurationMenu.SetReqBazCategory(categoryId);
        }
        else
        {
            Debug.LogError("RPC tried to change Requirements Bazaar project but Requirements Bazaar is not selected as source.", gameObject);
        }
    }

    [PunRPC]
    private async void SetGitHubOwner(short gitHubOwnerStringId)
    {
        string gitHubOwner = await NetworkedStringManager.GetString(gitHubOwnerStringId);
        Debug.Log("RPC: set GitHubOwner to " + gitHubOwner, gameObject);
        configurationMenu.SetGitHubOwner(gitHubOwner);
    }

    [PunRPC]
    private async void SetGitHubProject(short gitHubProjectStringId)
    {
        string gitHubProject = await NetworkedStringManager.GetString(gitHubProjectStringId);
        Debug.Log("RPC: set GitHubProject to " + gitHubProject, gameObject);
        configurationMenu.SetGitHubProject(gitHubProject);
    }

    [PunRPC]
    private void SetConfigWindow(bool open)
    {
        Debug.Log("RPC: set Configuration Window open to " + open);
        configurationMenu.SynchronizationInProgress = true;
        if (open)
        {
            configurationMenu.Open();
        }
        else
        {
            configurationMenu.Close();
        }
        configurationMenu.SynchronizationInProgress = false;
    }

    [PunRPC]
    private void SetPage(short page)
    {
        Debug.Log("RPC: set page to " + page);
        issueLoader.SynchronizationInProgress = true;
        issueLoader.Page = page;
        issueLoader.SynchronizationInProgress = false;
    }

    private void OnSourceChanged(object sender, EventArgs e)
    {
        photonView.RPC("SetSource", RpcTarget.Others, (byte)configurationMenu.ShelfConfiguration.SelectedSource);
    }

    private void OnReqBazProjectChanged(object sender, EventArgs e)
    {
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
        short gitHubOwnerStringId = await NetworkedStringManager.StringToId(configurationMenu.GitHubOwner);
        photonView.RPC("SetGitHubOwner", RpcTarget.Others, gitHubOwnerStringId);
    }

    private async void OnGitHubProjectChanged(object sender, EventArgs e)
    {
        short gitHubProjectStringId = await NetworkedStringManager.StringToId(configurationMenu.GitHubRepository);
        photonView.RPC("SetGitHubProject", RpcTarget.Others, gitHubProjectStringId);
    }

    private void OnConfigWindowOpened(object sender, EventArgs e)
    {
        photonView.RPC("SetConfigWindow", RpcTarget.Others, true);
    }

    private void OnConfigWindowClosed(object sender, EventArgs e)
    {
        photonView.RPC("SetConfigWindow", RpcTarget.Others, false);
    }

    private void OnPageChanged(object sender, EventArgs e)
    {
        photonView.RPC("SetPage", RpcTarget.Others, (short)issueLoader.Page);
    }
}
