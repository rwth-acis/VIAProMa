// -------------------------------------------------------------------------------------------------
// Main.cs
// Project: 3 Digits Redux
// Created: 2017/01/25
// Last Updated: 2018/08/21
// Copyright 2018 Dig-It! Games, LLC. All rights reserved.
// This code is licensed under the MIT License. (See LICENSE.txt for details)
//
// Note: Need to have RingBuffer.cs and TinCan included in the project.
// -------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;

using DIG.GBLXAPI;

public class Main : MonoBehaviour
{
	public Button clearButton;
	public Button submitButton;
	public Text statementText;
	public InputField hashField;
	public Button hashButton;

    private void Awake()
    {
		GBLXAPI.Init(new GBLConfig());

		GBLXAPI.debugMode = true;
		GBLXAPI.Timers.ResetSlot(0);
	}

    public void Start()
	{
		// Add listeners
		clearButton.onClick.AddListener(delegate { ClearButtonClicked(); });
		submitButton.onClick.AddListener(delegate { SubmitButtonClicked(); });
		hashButton.onClick.AddListener(delegate { HashButtonClicked(); });

		// Text box
		statementText.text = "";
	}

	private void HashButtonClicked()
	{
		GBL_Interface.userUUID = GBLUtils.GenerateActorUUID(hashField.text);
		statementText.text = GBL_Interface.userUUID;
	}

	private void ClearButtonClicked()
	{
		statementText.text = "";
	}

	// ------------------------------------------------------------------------
	// ------------------------------------------------------------------------
	private void SubmitButtonClicked()
	{
		GBL_Interface.SendContextStatement();

		statementText.text = "See the logs for your statement!";
	}
}
