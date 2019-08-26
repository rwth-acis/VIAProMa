using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssueShelfSynchronizer : TransformSynchronizer
{
    [SerializeField] private ShelfConfigurationMenu configurationMenu;

    private short gitHubOwnerStringId;
    private short gitHubProjectStringId;

    private void Awake()
    {
        if (configurationMenu == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(configurationMenu));
        }
    }

    private async void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            gitHubOwnerStringId = await NetworkedStringManager.RegisterStringResource();
            gitHubProjectStringId = await NetworkedStringManager.RegisterStringResource();

            photonView.RPC("SetStringIds", RpcTarget.Others, gitHubOwnerStringId, gitHubProjectStringId);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        photonView.RPC("SetActive", RpcTarget.Others, true);
        configurationMenu.SourceChanged += OnSourceChanged;
        configurationMenu.ReqBazProjectChanged += OnReqBazProjectChanged;
        configurationMenu.ReqBazCategoryChanged += OnReqBazCategoryChanged;
    }

    public override void OnDisable()
    {
        photonView.RPC("SetActive", RpcTarget.Others, false);
        configurationMenu.SourceChanged -= OnSourceChanged;
        base.OnDisable();
    }

    private void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            NetworkedStringManager.DeregisterStringResource(gitHubOwnerStringId);
            NetworkedStringManager.DeregisterStringResource(gitHubProjectStringId);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && newPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
        {
            // the master client informs the new player about the current status
            if (configurationMenu.ShelfConfiguration.SelectedSource == DataSource.REQUIREMENTS_BAZAAR)
            {
                ReqBazShelfConfiguration config = (ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration;
                photonView.RPC("Initialize", RpcTarget.Others,
                gameObject.activeSelf,
                (byte)config.SelectedSource,
                (short)config.SelectedProject.id,
                (short)config.SelectedCategory.id,
                gitHubOwnerStringId,
                gitHubProjectStringId
                );
            }
            else
            {
                photonView.RPC("Initialize", RpcTarget.Others,
                gameObject.activeSelf,
                (byte)configurationMenu.ShelfConfiguration.SelectedSource,
                (short)-1,
                (short)-1,
                gitHubOwnerStringId,
                gitHubProjectStringId
                );
            }
        }
    }

    [PunRPC]
    private void Initialize(
        bool activeState,
        byte sourceId,
        short reqBazProjectId,
        short reqBazCategoryId,
        short gitHubOwnerStringId,
        short gitHubProjectStringId
        )
    {
        // initializes the configuration
        SetActive(activeState);
        SetSource(sourceId);
        if (reqBazProjectId != -1)
        {
            SetReqBazProject(reqBazProjectId);
        }
        if (reqBazCategoryId != -1)
        {
            SetReqBazCategory(reqBazCategoryId);
        }
        this.gitHubOwnerStringId = gitHubOwnerStringId;
        this.gitHubProjectStringId = gitHubProjectStringId;

    }

    [PunRPC]
    private void SetStringIds(short gitHubOwnerStringId, short gitHubProjectStringId)
    {
        this.gitHubOwnerStringId = gitHubOwnerStringId;
        this.gitHubProjectStringId = gitHubProjectStringId;
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
    private async void SetGitHubOwner()
    {
        string gitHubOwner = await NetworkedStringManager.GetString(gitHubOwnerStringId);
        configurationMenu.SetGitHubOwner(gitHubOwner);
    }

    [PunRPC]
    private async void SetGitHubProject()
    {
        string gitHubProject = await NetworkedStringManager.GetString(gitHubProjectStringId);
        configurationMenu.SetGitHubProject(gitHubProject);
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

    private void OnGitHubOwnerChanged(object sender, EventArgs e)
    {
        photonView.RPC("SetGitHubOwner", RpcTarget.Others);
    }

    private void OnGitHubProjectChanged(object sender, EventArgs e)
    {
        photonView.RPC("SetGitHubProject", RpcTarget.Others);
    }
}
