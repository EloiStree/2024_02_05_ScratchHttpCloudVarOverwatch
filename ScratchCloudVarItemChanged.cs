using System;

[Serializable]
public class ScratchCloudVarItemChanged
{
    public string m_username;
    public string m_cloudVarName;
    public int m_value;
    public long m_scratchTimestamp;
    public DateTime m_nowUtc;
    internal string m_projectId;
} 

