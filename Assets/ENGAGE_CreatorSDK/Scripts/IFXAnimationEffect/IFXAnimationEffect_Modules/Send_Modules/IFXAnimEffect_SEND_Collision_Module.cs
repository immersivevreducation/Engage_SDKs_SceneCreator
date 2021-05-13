using UnityEngine;
[AddComponentMenu("IFXAnimEffect_SEND/Collision - IFX SEND Module")]
[RequireComponent(typeof(IFXAnimEffect_SEND_MAIN))]
public class IFXAnimEffect_SEND_Collision_Module : IFXAnimEffect_SEND_Module
{
    #if UNITY_EDITOR
    public override string moduleType {get{ return "Collision";} }
    #endif

    delegate float UpdateValuesDelegate();
    UpdateValuesDelegate UpdateValues;

    //////////////////////////////////
    [SerializeField]
    Collider collider1;
    [SerializeField]
    Collider collider2;

    //////////////////////////////////

    private void OnEnable()
    {
        if (collider1 != null && collider2 != null)
        {
            UpdateValues += GetCollision;
        }
        else
        {
            Debug.Log("AnimationEffect: from collision selected but Collider input empty");
        }
        
    }
    //This method gets called by SEND_Main to retrive to value from the delegate. Only one method should be returning values.
    public override void SendOutput()
    {
       AnimationEffectVariable.Value = UpdateValues();
    }
    ///////////////////////////////////////////////

    private float GetCollision() 
    {
        if(collider1.bounds.Intersects(collider2.bounds))
        {
            return 1.0f;
        }
        return 0.0f;
    }
    
}
