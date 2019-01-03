using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;
using System;
using System.Linq;

namespace Zframework.Editor
{   
    [CustomEditor(typeof(ProcedureManager))]
    internal class ProcedureManagerInspector:ZFrameworkInspector//TODO copy check
    {
        private SerializedProperty m_AvailableProcedureTypeNames = null;
        private SerializedProperty m_EntranceProcedureTypeName = null;
        /// <summary>从程序集读出来的 所有ProcedureBase的名字</summary>
        private string[] m_ProcedureTypeNames = null;
        private List<string> m_CurrentAvailableProcedureTypeNames = null;
        private int m_EntranceProcedureIndex = -1;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            ProcedureManager t = (ProcedureManager)target;

            if (string.IsNullOrEmpty(m_EntranceProcedureTypeName.stringValue))
            {
                EditorGUILayout.HelpBox("Entrance procedure is invalid.", MessageType.Error);
            }
            else if (EditorApplication.isPlaying)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Current Procedure", t.CurrentProcedure == null ? "None" : t.CurrentProcedure.GetType().ToString());
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Separator();
            //EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            //{
            //GUILayout.Label("Available Procedures", EditorStyles.boldLabel);
            if (m_ProcedureTypeNames.Length > 0)
                {
                    //EditorGUILayout.BeginVertical("box");
                    {
                        foreach (string procedureTypeName in m_ProcedureTypeNames)
                        {
                            //bool selected = m_CurrentAvailableProcedureTypeNames.Contains(procedureTypeName);
                            //if (selected != EditorGUILayout.ToggleLeft(procedureTypeName, selected))
                            //{
                            //    if (!selected)
                            //    {
                            //        m_CurrentAvailableProcedureTypeNames.Add(procedureTypeName);
                            //        WriteAvailableProcedureTypeNames();
                            //    }
                            //    else if (procedureTypeName != m_EntranceProcedureTypeName.stringValue)
                            //    {
                            //        m_CurrentAvailableProcedureTypeNames.Remove(procedureTypeName);
                            //        WriteAvailableProcedureTypeNames();
                            //    }
                            //}
                            if (!m_CurrentAvailableProcedureTypeNames.Contains(procedureTypeName))
                            {
                                m_CurrentAvailableProcedureTypeNames.Add(procedureTypeName);
                                WriteAvailableProcedureTypeNames();
                            }
                        }
                    }
                    //EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.HelpBox("There is no available procedure.", MessageType.Warning);
                }

                if (m_CurrentAvailableProcedureTypeNames.Count > 0)
                {
                    //EditorGUILayout.Separator();

                    int selectedIndex = EditorGUILayout.Popup("Entrance Procedure", m_EntranceProcedureIndex, m_CurrentAvailableProcedureTypeNames.ToArray());
                    if (selectedIndex != m_EntranceProcedureIndex)
                    {
                        m_EntranceProcedureIndex = selectedIndex;
                        m_EntranceProcedureTypeName.stringValue = m_CurrentAvailableProcedureTypeNames[selectedIndex];
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Select available procedures first.", MessageType.Info);
                }
            //}
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }

        private void OnEnable()
        {
            m_AvailableProcedureTypeNames = serializedObject.FindProperty("m_AvailableProcedureTypeNames");
            m_EntranceProcedureTypeName = serializedObject.FindProperty("m_EntranceProcedureTypeName");

            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_ProcedureTypeNames = Type.GetTypeNames(typeof(ProcedureBase));
            ReadAvailableProcedureTypeNames();
            int oldCount = m_CurrentAvailableProcedureTypeNames.Count;//从编辑器内读出当前选中的可选流程
            m_CurrentAvailableProcedureTypeNames = m_CurrentAvailableProcedureTypeNames.Where(x => m_ProcedureTypeNames.Contains(x)).ToList();//可能流程文件数量有减少
            if (m_CurrentAvailableProcedureTypeNames.Count != oldCount)
            {
                WriteAvailableProcedureTypeNames();
            }
            else if (!string.IsNullOrEmpty(m_EntranceProcedureTypeName.stringValue))
            {
                m_EntranceProcedureIndex = m_CurrentAvailableProcedureTypeNames.IndexOf(m_EntranceProcedureTypeName.stringValue);
                if (m_EntranceProcedureIndex < 0)
                {
                    m_EntranceProcedureTypeName.stringValue = null;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ReadAvailableProcedureTypeNames()
        {
            m_CurrentAvailableProcedureTypeNames = new List<string>();
            int count = m_AvailableProcedureTypeNames.arraySize;
            for (int i = 0; i < count; i++)
            {
                m_CurrentAvailableProcedureTypeNames.Add(m_AvailableProcedureTypeNames.GetArrayElementAtIndex(i).stringValue);
            }
        }

        private void WriteAvailableProcedureTypeNames()
        {
            m_AvailableProcedureTypeNames.ClearArray();
            if (m_CurrentAvailableProcedureTypeNames == null)
            {
                return;
            }

            m_CurrentAvailableProcedureTypeNames.Sort();
            int count = m_CurrentAvailableProcedureTypeNames.Count;
            for (int i = 0; i < count; i++)
            {
                m_AvailableProcedureTypeNames.InsertArrayElementAtIndex(i);
                m_AvailableProcedureTypeNames.GetArrayElementAtIndex(i).stringValue = m_CurrentAvailableProcedureTypeNames[i];
            }

            if (!string.IsNullOrEmpty(m_EntranceProcedureTypeName.stringValue))
            {
                m_EntranceProcedureIndex = m_CurrentAvailableProcedureTypeNames.IndexOf(m_EntranceProcedureTypeName.stringValue);
                if (m_EntranceProcedureIndex < 0)
                {
                    m_EntranceProcedureTypeName.stringValue = null;
                }
            }
        }
    }
}