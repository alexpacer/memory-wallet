package awei.memory.wallet;

import java.math.BigDecimal;

public class PlayerProfile {

    private String name;
    private BigDecimal amount;

    public PlayerProfile(String name){
        this.name = name;
        this.amount = new BigDecimal(0);
    }

    public String getName() { return name; }
    public void setName(String name) { this.name = name; }

    public BigDecimal getAmount() { return amount; }
    public void setAmount(BigDecimal amount) { this.amount = amount; }
}
