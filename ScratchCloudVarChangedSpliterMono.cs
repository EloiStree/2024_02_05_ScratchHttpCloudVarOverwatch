using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScratchCloudVarChangedSpliterMono : MonoBehaviour
{
    public string m_userCloudVarFormat = "{0}|{1}";
    public string m_userCloudVarInProjectFormat = "{0}|{1}|{2}";
    public string m_cloudVarInProjectFormat = "{0}|{1}";
    public int m_lastValueReceived;
    public UnityEvent<string, int> m_onUserValueReceived = new UnityEvent<string, int>();
    public UnityEvent<string, int> m_onCouldVarValueReceived = new UnityEvent<string, int>();
    public UnityEvent<string, int> m_onUserByCloudVarValueReceived = new UnityEvent<string, int>();
    public UnityEvent<string, int> m_onUserByCloudVarValueInProjectReceived = new UnityEvent<string, int>();
    public UnityEvent<string, int> m_onCloudVarValueInProjectReceived = new UnityEvent<string, int>();
    public UnityEvent<string,int> m_onProjectVarValueReceived = new UnityEvent<string, int>();
    public UnityEvent<int> m_onIntCmdReceived = new UnityEvent< int>();
    public void PushIn(ScratchCloudVarItemChanged value)
    {
        m_onUserValueReceived.Invoke(value.m_username, value.m_value);
        m_onCouldVarValueReceived.Invoke(value.m_cloudVarName, value.m_value);
        m_onUserByCloudVarValueReceived.Invoke(
            string.Format(m_userCloudVarFormat,
            value.m_username, value.m_cloudVarName), value.m_value);
        m_onUserByCloudVarValueInProjectReceived.Invoke(
            string.Format(m_userCloudVarInProjectFormat,
            value.m_username, value.m_cloudVarName, value.m_projectId), value.m_value);
        m_onCloudVarValueInProjectReceived.Invoke(
            string.Format(m_cloudVarInProjectFormat,
            value.m_cloudVarName, value.m_projectId), value.m_value);
        m_onProjectVarValueReceived.Invoke(value.m_projectId, value.m_value);
        m_onIntCmdReceived.Invoke(value.m_value);
        m_lastValueReceived = value.m_value;

    }

}
