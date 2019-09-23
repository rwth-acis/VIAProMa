using i5.ViaProMa.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommitStatisticsConfigurationWindow : MonoBehaviour, IWindow
{
    [Header("References")]
    [SerializeField] private CommitStatisticsVisualizer visualization;

    [Header("UI Elements")]
    [SerializeField] private InputField ownerInputField;
    [SerializeField] private InputField repositoryInputField;

    public bool WindowEnabled { get; set; }

    public bool WindowOpen { get; private set; }

    public event EventHandler WindowClosed;

    private void Awake()
    {
        if (visualization == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(visualization));
        }
        if (ownerInputField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(ownerInputField));
        }
        if (repositoryInputField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(repositoryInputField));
        }
    }

    private void Start()
    {
        ownerInputField.Text = visualization.Owner;
        repositoryInputField.Text = visualization.Repository;
        ownerInputField.TextChanged += OwnerChanged;
        repositoryInputField.TextChanged += RepositoryChanged;
    }

    private void OwnerChanged(object sender, EventArgs e)
    {
        repositoryInputField.Text = "";
        CheckConfiguration();
    }

    private void RepositoryChanged(object sender, EventArgs e)
    {
        CheckConfiguration();
    }

    private void CheckConfiguration()
    {
        visualization.Owner = ownerInputField.Text;
        visualization.Repository = repositoryInputField.Text;
        if (!string.IsNullOrWhiteSpace(ownerInputField.Text) && !string.IsNullOrWhiteSpace(repositoryInputField.Text))
        {
            visualization.UpdateView();
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }
}
