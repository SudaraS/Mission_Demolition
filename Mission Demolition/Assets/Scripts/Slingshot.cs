using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject ProjLinePrefab;
    public Transform leftBandAnchor;
    public Transform rightBandAnchor;
    public LineRenderer rubberBand;
    public AudioClip slingshotSnap;

    [Header("Dyanmic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    private AudioSource audioSource;

    void Awake(){
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        audioSource = GetComponent<AudioSource>();

        rubberBand.enabled = false;
    }
    void OnMouseEnter(){
        //print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }

    void OnMouseExit(){
        //print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }

    void OnMouseDown(){
        aimingMode = true;
        projectile = Instantiate(projectilePrefab) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;

        if(slingshotSnap != null && audioSource != null){
                audioSource.PlayOneShot(slingshotSnap);
        }

        rubberBand.enabled = true;
    }

    void Update(){
        if(!aimingMode) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D -launchPos;

        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if(mouseDelta.magnitude > maxMagnitude){
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        UpdateRubberBand(projPos);

        if(Input.GetMouseButtonUp(0)){
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            /*if(slingshotSnap != null && audioSource != null){
                audioSource.PlayOneShot(slingshotSnap);
            }*/

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
            
            FollowCam.POI = projectile;
            Instantiate<GameObject>(ProjLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();

            rubberBand.enabled = false;
        }
    }

    void UpdateRubberBand(Vector3 projectilePos){
        rubberBand.positionCount = 3;

        rubberBand.SetPosition(0, leftBandAnchor.position);
        rubberBand.SetPosition(1, projectilePos);
        rubberBand.SetPosition(2, rightBandAnchor.position);
    }
}
