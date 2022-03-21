using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    [SerializeField] private GameObject currentWeapon;

    private void Start()
    {
        currentWeapon = Instantiate(currentWeapon, transform.position, Quaternion.identity);
        currentWeapon.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentWeapon.GetComponent<Activatable>().Activate(this.gameObject);
        }
        
    }
}
