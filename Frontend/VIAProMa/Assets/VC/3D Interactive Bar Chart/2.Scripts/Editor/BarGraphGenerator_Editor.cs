using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

namespace BarGraph.VittorCloud
{
#if UNITY_EDITOR

    [CustomEditor(typeof(BarGraphGenerator))]
    public class BarGraphGenerator_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            BarGraphGenerator script = (BarGraphGenerator)target;
            //DrawDefaultInspector();
            //OnHeaderGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Graph Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            script.MaxHeight = (int)EditorGUILayout.IntField("Max Height", script.MaxHeight = 10);
            script.xStart = EditorGUILayout.FloatField("X Start", script.xStart);
            script.yStart = EditorGUILayout.FloatField("Y Start", script.yStart);
            script.zStart = EditorGUILayout.FloatField("Z Start", script.zStart);

            script.segmentSizeOnXaxis = EditorGUILayout.FloatField("Segment Size On X axis", script.segmentSizeOnXaxis);
            script.segmentSizeOnYaxis = EditorGUILayout.FloatField("Segment Size On Y axis", script.segmentSizeOnYaxis);
            script.segmentSizeOnZaxis = EditorGUILayout.FloatField("Segment Size On Z axis", script.segmentSizeOnZaxis);

            script.offsetBetweenXRow = EditorGUILayout.FloatField("Offset Between X Row", script.offsetBetweenXRow);
            script.offsetBetweenZRow = EditorGUILayout.FloatField("Offset Between Z Row", script.offsetBetweenZRow);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Graph Animation Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            script.animationSpeed = EditorGUILayout.Slider("Animation Speed", script.animationSpeed, 0f, 15f);

            SerializedProperty graphAnimation = serializedObject.FindProperty(GetMemberName(() => script.graphAnimation));
            EditorGUILayout.PropertyField(graphAnimation);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Bar Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            SerializedProperty barPrefabe = serializedObject.FindProperty(GetMemberName(() => script.barPrefab));
            EditorGUILayout.PropertyField(barPrefabe);
            SerializedProperty barColorProperty = serializedObject.FindProperty(GetMemberName(() => script.barColorProperty));
            EditorGUILayout.PropertyField(barColorProperty);

            if (script.graphAnimation == BarGraphGenerator.AnimatioType.animationWithGradient)
                script.barColorProperty = BarGraphGenerator.BarColor.HeightWiseGradient;

            if (script.barColorProperty == BarGraphGenerator.BarColor.HeightWiseGradient)
            {
                SerializedProperty HeightWiseGredient = serializedObject.FindProperty(GetMemberName(() => script.HeightWiseGradient));
                EditorGUILayout.PropertyField(HeightWiseGredient);
            }
            script.barScaleFactor = EditorGUILayout.FloatField("Bar Scale Factor", script.barScaleFactor);

            SerializedProperty GraphRef = serializedObject.FindProperty(GetMemberName(() => script.GraphRef));
            EditorGUILayout.PropertyField(GraphRef);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Custom Events", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("The Following Events will pass the effected bar as a gameobject.", EditorStyles.helpBox);

            SerializedProperty OnBarPointerDown = serializedObject.FindProperty(GetMemberName(() => script.OnBarPointerDown));
            EditorGUILayout.PropertyField(OnBarPointerDown);

            SerializedProperty OnBarPointerUp = serializedObject.FindProperty(GetMemberName(() => script.OnBarPointerUp));
            EditorGUILayout.PropertyField(OnBarPointerUp);

            SerializedProperty OnBarHoverEnter = serializedObject.FindProperty(GetMemberName(() => script.OnBarHoverEnter));
            EditorGUILayout.PropertyField(OnBarHoverEnter);

            SerializedProperty OnBarHoverExit = serializedObject.FindProperty(GetMemberName(() => script.OnBarHoverExit));
            EditorGUILayout.PropertyField(OnBarHoverExit);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("This will be Invoked when the starting animation of the graph is completed.", EditorStyles.helpBox);
            SerializedProperty OnInitialGraphCompleted = serializedObject.FindProperty(GetMemberName(() => script.OnInitialGraphCompleted));
            EditorGUILayout.PropertyField(OnInitialGraphCompleted);



            serializedObject.ApplyModifiedProperties();
        }

        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }
    }
#endif
}