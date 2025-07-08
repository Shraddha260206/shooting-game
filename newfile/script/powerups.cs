using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class powerups : MonoBehaviour
{
    [SerializeField] private int powerup_id;

    [SerializeField] private AudioClip audioClip;
  
    void Update()
    {
        transform.Translate(Vector3.down*3*Time.deltaTime);
        if (transform.position.y < -6.321f)
        {
             Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other){
        if (other.tag== "player"){
            player player= other.transform.GetComponent<player>();
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
            if (player != null)
            {
                switch (powerup_id)
                {
                    case 1:
                        player.speedBoastActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;


                }
            }
            Destroy(this.gameObject);

        }
       
    }

}