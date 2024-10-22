using UnityEngine;

public class CreditsUI : MonoBehaviour
{
    [System.Serializable]
    public struct CreditEntry
    {
        public string MemberName;
        public string MemberRoles;
        public string LinkURL;
        public Sprite MemberIcon;
    }

    [field: SerializeField] private GameObject Parent;
    [field: SerializeField] private CreditEntryUI Template;
    [field: SerializeField] private CreditEntry[] Prefabs;

    private void Start()
    {
        int index = 0;

        foreach (CreditEntry prefab in Prefabs)
        {
            CreditEntryUI entry = Instantiate(Template);
            
            entry.MemberName = prefab.MemberName;
            entry.MemberRoles = prefab.MemberRoles;
            entry.LinkURL = prefab.LinkURL;
            entry.MemberIcon = prefab.MemberIcon;

            entry.name = "Credit_" + (index + 1).ToString();
            entry.transform.SetParent(Parent.transform, false);
            entry.gameObject.SetActive(true);

            index++;
        }
    }
}
