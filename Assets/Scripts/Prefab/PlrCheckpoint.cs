using UnityEngine;

public class PlrCheckpoint : MonoBehaviour
{
    [field: Space(2.5f), SerializeField] private string OverrideName;
    [field: SerializeField] private Transform OverrideTranform;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        SaveData data = DataHandler.Instance.GetCachedData();

        if (data.checkpointName == gameObject.name)
        {
            Debug.LogWarning(name + " | You've already registered a Checkpoint with the name: " + gameObject.name + "! Please use a different Checkpoint to save!");
            return;
        }

        data.checkpointName = (!string.IsNullOrWhiteSpace(OverrideName)) ? OverrideName : gameObject.name;

        Transform target = (OverrideTranform != null) ? OverrideTranform : transform;
        data.checkpointPosition = DataHandler.Instance.ConvertVector3ToFloatArray(target.position);
        data.checkpointRotation = DataHandler.Instance.ConvertVector3ToFloatArray(target.rotation.eulerAngles);

        DataHandler.Instance.SetCachedData(data);
        bool result = DataHandler.Instance.SaveCachedDataToFile();

        if (result) Debug.Log(name + " Successfully saved: " + DataHandler.Instance.GetFileName() + " to disk!");
        else Debug.LogWarning(name +  "Failed to save: " + DataHandler.Instance.GetFileName() + " to disk... :(");
    }
}
