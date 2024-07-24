using PSGJ15_DCSA.Enums;

namespace PSGJ15_DCSA.Interfaces
{
    public interface ICharacterStats
    {
        public void Initialize();
        public int GetMaxHealthValue();
        public void SetMaxHealthValue(int value);
        public int GetHealthValue();
        public void SetHealthValue(int value);
        public float GetMovementSpeedValue();
        public void SetMovementSpeedValue(float value);
        public int GetDamageValue();
        public void SetDamageValue(int value);
        public ElementTypes GetElementAffinity();
        public void SetElementAffinity(ElementTypes element);
        public void ResetAllValues();
    }
}
