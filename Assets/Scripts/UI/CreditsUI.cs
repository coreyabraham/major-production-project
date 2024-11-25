using UnityEngine;

public class CreditsUI : MonoBehaviour
{
    [System.Serializable]
    public struct CreditEntry
    {
        public string MemberName;
        public string MemberRoles;
        public string MemberLink;

        public Sprite MemberIcon;

        public CreditEntryUI Instance;
    }

    [field: SerializeField] private CreditEntry[] Prefabs;

    private void Awake()
    {
        for (int i = 0; i < Prefabs.Length; i++)
        {
            Prefabs[i].Instance.MemberName = Prefabs[i].MemberName;
            Prefabs[i].Instance.MemberRoles = Prefabs[i].MemberRoles;
            Prefabs[i].Instance.MemberLink = Prefabs[i].MemberLink;
            Prefabs[i].Instance.MemberIcon = Prefabs[i].MemberIcon;
        }
    }
}