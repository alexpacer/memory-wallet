class Player {

  var balance: BigDecimal = 0

  def deposit(amt: BigDecimal): BigDecimal = {
    balance += amt
    balance
  }

  def withdraw(amt: BigDecimal): BigDecimal =
    if (balance < amt) throw new Error("Insufficient fund")
    else {
      balance -= amt
      balance
    }
}
