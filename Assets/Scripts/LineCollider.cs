using UnityEngine;

public class LineCollider : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            BuildingController origin = transform.parent.GetComponent<LineProperties>().lineOrigin;

            if (origin.type == BuildingController.Type.Opponent || origin.type == BuildingController.Type.Opponent2)
            {
                origin.freeBuildings.Remove(transform.parent.GetComponent<LineProperties>().lineTarget.gameObject
                    .transform);
            }

            origin.createdLines.Remove(transform.parent.gameObject);
            
            if(origin.numberOfCreatedLines > 0)
              origin.numberOfCreatedLines--;
            
            origin.aiStopCreateLines = false;

            Destroy(transform.parent.gameObject);
        }
    }
}
