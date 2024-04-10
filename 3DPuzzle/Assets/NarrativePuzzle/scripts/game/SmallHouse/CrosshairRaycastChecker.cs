using UnityEngine;

public class CrosshairRaycastChecker : MonoBehaviour
{

    public LayerMask checkLayer;
    private CrosshairRaycastCheckTarget _crtCrct;

    public void Update()
    {
        var rayStart = transform.position;
        Vector3 direction = transform.forward;
        //Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask,
        RaycastHit ray;
        if (Physics.Raycast(rayStart, direction, out ray, 3, checkLayer))
        {
            Debug.Log(ray.collider);
            if (ray.collider != null)
            {
                Debug.Log("shoot hit!");
                var crct = ray.collider.GetComponent<CrosshairRaycastCheckTarget>();
                if (crct != null)
                {
                    if (_crtCrct==null)
                    {
                        //new crct entered!
                    }else if (_crtCrct==crct)
                    {
                        //same crct
                    }
                    else
                    {
                        //another crct
                        //accept the new one!
                    }
                    _crtCrct=crct;
                    crct.OnEnter();
                }
                else
                {

                }
            }
        }
    }
}