using UnityEngine;

public interface IMotor 
{
   public void Move(MotorContext ctx);

   public void Rotate(MotorContext ctx);
}
