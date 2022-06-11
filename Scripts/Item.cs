using UnityEngine;
//¹ð±Û¹ð±Û ¾ÆÀÌÅÛ
public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon, MaxHeart, MaxAmmo, MaxRapid, MaxSpeed, MaxAttack };
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 25 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }
}
