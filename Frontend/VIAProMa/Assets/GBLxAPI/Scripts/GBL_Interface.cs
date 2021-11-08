// -------------------------------------------------------------------------------------------------
// GBL_Interface.cs
// Project: GBLXAPI-Unity
// Created: 2018/07/06
// Copyright 2018 Dig-It! Games, LLC. All rights reserved.
// This code is licensed under the MIT License (see LICENSE.txt for details)
// -------------------------------------------------------------------------------------------------
using DIG.GBLXAPI;
using TinCan;
using System.Collections.Generic;

// --------------------------------------------------------------------------------------
// --------------------------------------------------------------------------------------
public static class GBL_Interface
{
	public static string userUUID = "test";

	private static Agent PlayerAgent => GBLXAPI.Agent
		.WithAccount(userUUID, "https://dig-itgames.com/")
		.WithName("Test User")
		.Build();

	public static void SendContextStatement()
	{
		GBLXAPI.Statement
			.WithActor(PlayerAgent)
			.WithVerb("pressed")
			.WithTargetActivity(GBLXAPI.Activity
				.WithID("https://dig-itgames.com/apps/GBLXAPITEST")
				.WithType("serious-game")
				.WithValue("GBLXAPI TEST")
				.Build())
			.WithContext(CreateTestContext())
			.Enqueue();
	}

	public static void SendTimerStarted()
    {
		GBLXAPI.Statement
			.WithActor(PlayerAgent)
			.WithVerb("started")
			.WithTargetActivity(GBLXAPI.Activity
				.WithID("https://dig-itgames.com/apps/GBLXAPITEST")
				.WithType("serious-game")
				.WithValue("GBLXAPI TEST")
				.Build())
			.WithContext(GBLXAPI.Context
				.WithParents(new List<Activity>
				{
					GBLXAPI.Activity
						.WithID("https://dig-itgames.com/apps/GBLXAPITEST")
						.WithType("serious-game")
						.WithValue("GBLXAPI TEST")
						.Build()
				})
				.WithGroupings(new List<Activity>
				{
					GBLXAPI.Activity.WithID("https://dig-itgames.com/").Build()
				})
				.Build())
			.Enqueue();
	}

	public static void SendTimerStopped()
    {
		GBLXAPI.Statement
			.WithActor(PlayerAgent)
			.WithVerb("completed")
			.WithTargetActivity(GBLXAPI.Activity
				.WithID("https://dig-itgames.com/apps/GBLXAPITEST")
				.WithType("serious-game")
				.WithValue("GBLXAPI TEST")
				.Build())
			.WithContext(GBLXAPI.Context
				.WithParents(new List<Activity>
				{
					GBLXAPI.Activity
						.WithID("https://dig-itgames.com/apps/GBLXAPITEST")
						.WithType("serious-game")
						.WithValue("GBLXAPI TEST")
						.Build()
				})
				.WithGroupings(new List<Activity>
				{
					GBLXAPI.Activity.WithID("https://dig-itgames.com/").Build()
				})
				.Build())
			.WithResult(GBLXAPI.Result
				.Complete()
				.Successful()
				.WithDuration(GBLXAPI.Timers.GetSlot(1)))
			.Enqueue();
	}

	// // ------------------------------------------------------------------------
	// // Sample Context Generators
	// // ------------------------------------------------------------------------
    /*
    Since context generation can be many lines of code, it is often helpful to separate it out into helper functions. 
    These functions will be responsible for creating Context Activities, Context Extensions, and assigning them to a singular Context object.
     */
	public static Context CreateTestContext()
	{
		return GBLXAPI.Context
			.WithParents(new List<Activity>
			{
				GBLXAPI.Activity
					.WithID("https://company.com/example-game")
					.WithType("serious-game")
					.WithValue("GBLXAPI TEST")
					.Build()
			})
			.WithGroupings(new List<Activity>
			{
				GBLXAPI.Activity.WithID("https://company.com/").Build()
			})
			.WithCategories(new List<Activity>
			{
				GBLXAPI.Activity.WithID("https://gblxapi.org/socialstudies").Build(),
				GBLXAPI.Activity.WithID("https://gblxapi.org/math").Build()
			})
			.WithExtensions(GBLXAPI.Extensions
				.WithStandard("Grade", "Grade 4 level")
				.WithStandard("Domain", "History")
				.WithStandard("Domain", "Number and Operations in Base Ten")
				.WithStandard("Subdomain", "Problem Solving")
				.WithStandard("Skill", "Patterns and Relationships")
				.WithStandard("Skill", "Calculation and Computation")
				.WithStandard("Topic", "Arithmetic")
				.WithStandard("Focus", "Addition/Subtraction")
				.WithStandard("Action", "Solve Problems")
				.WithStandard("C3 Framework", "d2.His.13.6-8.", "c3")
				.WithStandard("CC-MATH", "CCSS.Math.Content.4.NBT.B.4", "cc")
				.WithStandard("CC-MATH", "CCSS.Math.Content.5.NBT.A.1", "cc")
				.Build())
			.Build();
	}
}