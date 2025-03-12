/* BUG: Transform object of Rigidbody in Update() methods
* MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
*         public override void Update()
*         {
*             base.Update();
* 
*             Debug.DrawRay(transform.position, transform.forward, Color.red, 0.1f);
* 
*             DistanceGrabbable target;
*             Collider targetColl;
*             FindTarget(out target, out targetColl);
* 
*             if (target != m_target)
*             {
*                 if (m_target != null)
*                 {
*                     m_target.Targeted = m_otherHand.m_target == m_target;
*                 }
* 
*                 m_target = target;
*                 m_targetCollider = targetColl;
*                 if (m_target != null)
*                 {
*                     m_target.Targeted = true;
*                 }
*             }
*         }
