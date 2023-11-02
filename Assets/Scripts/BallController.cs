using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class BallController : MonoBehaviour
{
    // Start is called before the first frame update
    
    public Rigidbody rb;
    public float speed = 1;
    private bool isTraveling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;
    public int minSwipeRecognition = 500;
    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 currentSwipe;

    private Color solveColor;
    public AudioSource wallSound;
    public ParticleSystem movementParticle;

    private void Start()
    {
        solveColor = Random.ColorHSV(0.5f,1);
        GetComponent<MeshRenderer>().material.color = solveColor;
       /* movementParticle = GetComponent<ParticleSystem>();*/
    }

    private void FixedUpdate()
    {
        if (isTraveling)
        {
            rb.velocity = speed * travelDirection;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up/2), 0.05f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();
            if (ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor);
            }
            i++;
        }

        if(nextCollisionPosition != Vector3.zero)
        {
            if(Vector2.Distance(transform.position, nextCollisionPosition) < 1)
            {
                isTraveling = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
            }
        }


        if(isTraveling)
            return;

        if(Input.GetMouseButton(0))
        {
            swipePosCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if (swipePosLastFrame != Vector2.zero)
            {
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                if(currentSwipe.sqrMagnitude < minSwipeRecognition)
                {
                    return;
                }

                currentSwipe.Normalize();


                //UP/DOWN
                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f )
                {
                    //GO UP/DOWN
                    SetDestination(currentSwipe.y >0 ? Vector3.forward : Vector3.back);
                    movementParticle.Play();
                }

                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f )
                {
                    //GO LEFT/RIGHT
                    SetDestination(currentSwipe.x >0 ? Vector3.right : Vector3.left);
                    movementParticle.Play();
                }
            }
              

            swipePosLastFrame = swipePosCurrentFrame;
        }  


        if(Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }

}


         private void SetDestination(Vector3 direction)
        {
            travelDirection = direction;
            
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, 100f))
            {
                nextCollisionPosition = hit.point;
            }

            isTraveling = true;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Wall")
            {
                wallSound.Play();
                
            }

            
        }
    



    
}
