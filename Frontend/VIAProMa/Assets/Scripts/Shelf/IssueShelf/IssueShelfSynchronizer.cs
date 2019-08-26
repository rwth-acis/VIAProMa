using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssueShelfSynchronizer : TransformSynchronizer
{
    [SerializeField] private ShelfConfigurationMenu configurationMenu;

    private void Awake()
    {
        if (configurationMenu == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(configurationMenu));
        }
    }

    private void OnEnable()
    {
        photonView.RPC("SetActive", RpcTarget.Others, true);
        configurationMenu.SourceChanged += OnSourceChanged;
        configurationMenu.ReqBazProjectChanged += OnReqBazProjectChanged;
        configurationMenu.ReqBazCategoryChanged += OnReqBazCategoryChanged;
    }

    private void OnDisable()
    {
        photonView.RPC("SetActive", RpcTarget.Others, false);
        configurationMenu.SourceChanged -= OnSourceChanged;
    }

    [PunRPC]
    public void SetActive(bool active)
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
}
