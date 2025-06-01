using UnityEngine;

public class Deathbox : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private ETeam teamToKill;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other)
        {
            IUnit otherUnit = other.GetComponent<IUnit>();
            if (otherUnit != null && otherUnit.GetTeam() == this.teamToKill)
            {
                Destroy(other.gameObject);
            }
        }
    }
}
