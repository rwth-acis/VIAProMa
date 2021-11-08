using DisruptorUnity3d;
using System;
using System.Collections;
using TinCan;
using UnityEngine;

namespace DIG.GBLXAPI.Internal
{
    public class LrsRemoteQueue : MonoBehaviour
    {
		public const string GAMEOBJECT_NAME = "GBLXAPI";

		// ************************************************************************
		// Monobehaviour singleton
		// ************************************************************************
		private static LrsRemoteQueue instance = null;
		public static LrsRemoteQueue Instance
		{
			get
			{
				if (instance == null)
				{
					instance = (new GameObject(GAMEOBJECT_NAME)).AddComponent<LrsRemoteQueue>();
				}

				return instance;
			}
		}

		public bool useDefaultCallback = true;

		public bool ReadyToSend { get; private set; }

		private RemoteLRSAsync _lrsEndpoint; // WebGL/Desktop/Mobile coroutine implementation of RemoteLRS.cs

		private RingBuffer<QueuedStatement> _statementQueue;

		// ------------------------------------------------------------------------
		// Set singleton so it persists across scene loads
		// ------------------------------------------------------------------------
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		public void Init(GBLConfig config, int queueDepth = 1000)
		{
			_lrsEndpoint = new RemoteLRSAsync(GBLConfig.LrsURL, config.lrsUser, config.lrsPassword);
			_statementQueue = new RingBuffer<QueuedStatement>(queueDepth);

			ReadyToSend = true;
		}

		private void Update()
		{
			// GBL is open/ready to send?
			if (!ReadyToSend || _statementQueue == null || _statementQueue.Count == 0) { return; }

			// TODO: Remove this weird stop / start coroutine sequence
			StopAllCoroutines();
			StartCoroutine(SendStatementCoroutine());
		}

		public void EnqueueStatement(Statement statement, Action<bool, string> sendCallback = null)
		{
			// Make sure all required fields are set
			bool valid = true;
			string invalidReason = "";
			if (statement.actor == null) { valid = false; invalidReason += "ERROR: Agent is null\n"; }
			if (statement.verb == null) { valid = false; invalidReason += "ERROR: Verb is null\n"; }
			if (statement.target == null) { valid = false; invalidReason += "ERROR: Object is null\n"; }

			// Use default callback if none was given
			if (sendCallback == null && useDefaultCallback)
			{
				sendCallback = StatementDefaultCallback;
			}

			if (valid)
			{
				// Check if space in the ringbuffer queue, if not discard or will hard lock unity
				if (_statementQueue.Capacity - _statementQueue.Count > 0)
				{
					_statementQueue.Enqueue(new QueuedStatement(statement, sendCallback));
				}
				else
				{
					Debug.LogWarning("QueueStatement: Queue is full. Discarding Statement");
				}
			}
			else
			{
				sendCallback?.Invoke(false, invalidReason);
			}
		}

		// ------------------------------------------------------------------------
		// This coroutine spawns a thread to send the statement to the LRS
		// ------------------------------------------------------------------------
		private IEnumerator SendStatementCoroutine()
		{
			// Lock
			ReadyToSend = false;

			// Dequeue statement if exists in queue
			if (_statementQueue.TryDequeue(out QueuedStatement queuedStatement))
			{
				// Debug statement
				if (GBLXAPI.debugMode)
				{
					Debug.Log(queuedStatement.statement.ToJSON(true));
				}

				_lrsEndpoint.PostStatement(queuedStatement.statement);

				// Wait for the coroutine to finish
				while (!_lrsEndpoint.complete) { yield return null; }

				// Client callback with result
				queuedStatement.callback?.Invoke(_lrsEndpoint.success, _lrsEndpoint.response);
			}

			// Unlock
			ReadyToSend = true;
		}

		private void StatementDefaultCallback(bool result, string resultText)
		{
			if (result) { Debug.Log("GBLXAPI: SUCCESS: " + resultText); }
			else { Debug.Log("GBLXAPI: ERROR: " + resultText); }
		}

		public struct QueuedStatement
		{
			public Statement statement;
			public Action<bool, string> callback;

			public QueuedStatement(Statement statement, Action<bool, string> callback)
			{
				this.statement = statement;
				this.callback = callback;
			}
		}
	}
}
