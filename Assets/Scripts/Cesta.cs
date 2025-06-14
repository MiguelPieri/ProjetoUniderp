using Unity.VisualScripting;
using UnityEngine;

public class Cesta : MonoBehaviour
{
    public float multiplicadorForca = 10f;
    public float velocidadeMaxima = 3f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        var horizontalInput = Input.GetAxis("Horizontal");
        var vetrticalInput = Input.GetAxis("Vertical");
        if (GetComponent<Rigidbody>().linearVelocity.x <= velocidadeMaxima)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(horizontalInput * multiplicadorForca, 0, vetrticalInput * multiplicadorForca));

        }
        ;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            Destroy(gameObject);
            
        
    }

}
