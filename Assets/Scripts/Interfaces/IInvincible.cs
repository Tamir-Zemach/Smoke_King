namespace Interfaces
{
    public interface IInvincible
    {
        bool IsInvincible { get; set; }
        void OnInvincibleStart();
        void OnInvincibleEnd();
    }
}