public class HealthSystem
{
    private int currentHealth;
    private int maxHealth;

    public HealthSystem(int maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void Damage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth < 0)
        {
            currentHealth = 0;
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
