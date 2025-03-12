*     private void Throw()
*     {
*         readyToThrow = false;
* 
*         if (throwCounter < totalThrows)
*         {
*             audioManager.PlaySFX(audioManager.axeSound);
* 
*             GameObject thrownObject = Instantiate(objectToThrow, attackPoint.position, attackPoint.rotation);
*             Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
*             rb.AddForce(cam.forward * throwForce, ForceMode.VelocityChange);
*             throwCounter++;
*         }
*         else if (throwCounter == totalThrows)
*         {
*             audioManager.PlaySFX(audioManager.disappearSound);
*             armorObject.SetActive(false); // Make the armor disappear
*             audioManager.PlaySFX(audioManager.appearSound);
*             swordObject.SetActive(true); // Make the sword appear
*         }
* 
*         totalThrows--;
* 
* 
*         Invoke(nameof(ResetThrow), throwCooldown);
*     }

* FIXED CODE:
*/

* Move this function in FixedUpdate() methods.