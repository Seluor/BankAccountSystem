using System;
using System.Collections.Generic;

// Интерфейс для оповещения подписчиков об изменениях в счете
public interface IAccountObservable {
    void Subscribe(IAccountObserver observer);   // Подписаться на оповещения
    void Unsubscribe(IAccountObserver observer); // Отписаться от оповещений
    void Notify();                              // Оповестить подписчиков
}

// Интерфейс для подписчиков (пользователей)
public interface IAccountObserver {
    void Update(Account account); // Обновить состояние счета
}

// Реализация счета
public class Account : IAccountObservable {
    private static int nextId = 1;
    private List<IAccountObserver> observers; // Список подписчиков
    private decimal balance;                  // Баланс счета

    public int Id { get; }

    public Account(decimal initialBalance) {
        observers = new List<IAccountObserver>();
        balance = initialBalance;
        Id = nextId++;
    }

    public decimal GetBalance() {
        return balance;
    }

    public void Deposit(decimal amount) {
        balance += amount;
        Notify();
    }

    public void Withdraw(decimal amount) {
        if (amount <= balance) {
            balance -= amount;
            Notify();
        }
    }

    public void Transfer(decimal amount, Account targetAccount) {
        if (amount <= balance) {
            balance -= amount;
            targetAccount.Deposit(amount);
            Notify();
        }
    }

    public void Subscribe(IAccountObserver observer) {
        observers.Add(observer);
    }

    public void Unsubscribe(IAccountObserver observer) {
        observers.Remove(observer);
    }

    public void Notify() {
        foreach (var observer in observers) {
            observer.Update(this);
        }
    }
}

// Реализация пользователя
public class User : IAccountObserver {
    private string name;

    public User(string name) {
        this.name = name;
    }

    public void Update(Account account) {
        Console.WriteLine($"User {name}: Account balance changed to {account.GetBalance()}");
    }

    public string GetName() {
        return name;
    }
}

// Главное меню
public class MainMenu {
    private List<User> users;
    private List<Account> accounts;

    public MainMenu() {
        users = new List<User>();
        accounts = new List<Account>();
    }

    public void Start() {
        while (true) {
            Console.WriteLine("\n========== Bank System ==========");
            Console.WriteLine("1. Create User");
            Console.WriteLine("2. Create Account");
            Console.WriteLine("3. Deposit");
            Console.WriteLine("4. Withdraw");
            Console.WriteLine("5. Transfer");
            Console.WriteLine("6. Check Balance");
            Console.WriteLine("7. Exit");
            Console.Write("Enter option number: ");

            int option;
            if (int.TryParse(Console.ReadLine(), out option)) {
                switch (option) {
                    case 1:
                    CreateUser();
                    break;
                    case 2:
                    CreateAccount();
                    break;
                    case 3:
                    Deposit();
                    break;
                    case 4:
                    Withdraw();
                    break;
                    case 5:
                    Transfer();
                    break;
                    case 6:
                    CheckBalance();
                    break;
                    case 7:
                    Console.WriteLine("Exiting...");
                    return;
                    default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
                }
            }
            else {
                Console.WriteLine("Invalid input. Please try again.");
            }
        }
    }

    private void CreateUser() {
        Console.Write("Enter user name: ");
        string name = Console.ReadLine();
        User user = new User(name);
        users.Add(user);
        Console.WriteLine($"User {name} created successfully.");
    }

    private void CreateAccount() {
        if (users.Count == 0) {
            Console.WriteLine("No users found. Please create a user first.");
            return;
        }

        Console.Write("Enter user name: ");
        string name = Console.ReadLine();
        User user = users.Find(u => u.GetName() == name);

        if (user == null) {
            Console.WriteLine("User not found.");
            return;
        }

        Console.Write("Enter initial balance: ");
        decimal balance;
        if (decimal.TryParse(Console.ReadLine(), out balance)) {
            Account account = new Account(balance);
            account.Subscribe(user);
            accounts.Add(account);
            Console.WriteLine("Account created successfully.");
        }
        else {
            Console.WriteLine("Invalid balance amount.");
        }
    }

    private void Deposit() {
        if (accounts.Count == 0) {
            Console.WriteLine("No accounts found. Please create an account first.");
            return;
        }

        Console.Write("Enter account ID: ");
        int accountId;
        if (int.TryParse(Console.ReadLine(), out accountId)) {
            Account account = accounts.Find(a => a.Id == accountId);

            if (account == null) {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.Write("Enter deposit amount: ");
            decimal amount;
            if (decimal.TryParse(Console.ReadLine(), out amount)) {
                account.Deposit(amount);
                Console.WriteLine("Deposit successful.");
            }
            else {
                Console.WriteLine("Invalid deposit amount.");
            }
        }
        else {
            Console.WriteLine("Invalid account ID.");
        }
    }

    private void Withdraw() {
        if (accounts.Count == 0) {
            Console.WriteLine("No accounts found. Please create an account first.");
            return;
        }

        Console.Write("Enter account ID: ");
        int accountId;
        if (int.TryParse(Console.ReadLine(), out accountId)) {
            Account account = accounts.Find(a => a.Id == accountId);

            if (account == null) {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.Write("Enter withdrawal amount: ");
            decimal amount;
            if (decimal.TryParse(Console.ReadLine(), out amount)) {
                account.Withdraw(amount);
                Console.WriteLine("Withdrawal successful.");
            }
            else {
                Console.WriteLine("Invalid withdrawal amount.");
            }
        }
        else {
            Console.WriteLine("Invalid account ID.");
        }
    }

    private void Transfer() {
        if (accounts.Count < 2) {
            Console.WriteLine("At least two accounts are required to perform a transfer.");
            return;
        }

        Console.Write("Enter source account ID: ");
        int sourceAccountId;
        if (int.TryParse(Console.ReadLine(), out sourceAccountId)) {
            Account sourceAccount = accounts.Find(a => a.Id == sourceAccountId);

            if (sourceAccount == null) {
                Console.WriteLine("Source account not found.");
                return;
            }

            Console.Write("Enter target account ID: ");
            int targetAccountId;
            if (int.TryParse(Console.ReadLine(), out targetAccountId)) {
                Account targetAccount = accounts.Find(a => a.Id == targetAccountId);

                if (targetAccount == null) {
                    Console.WriteLine("Target account not found.");
                    return;
                }

                Console.Write("Enter transfer amount: ");
                decimal amount;
                if (decimal.TryParse(Console.ReadLine(), out amount)) {
                    sourceAccount.Transfer(amount, targetAccount);
                    Console.WriteLine("Transfer successful.");
                }
                else {
                    Console.WriteLine("Invalid transfer amount.");
                }
            }
            else {
                Console.WriteLine("Invalid target account ID.");
            }
        }
        else {
            Console.WriteLine("Invalid source account ID.");
        }
    }

    private void CheckBalance() {
        if (accounts.Count == 0) {
            Console.WriteLine("No accounts found. Please create an account first.");
            return;
        }

        Console.Write("Enter account ID: ");
        int accountId;
        if (int.TryParse(Console.ReadLine(), out accountId)) {
            Account account = accounts.Find(a => a.Id == accountId);

            if (account == null) {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.WriteLine($"Account balance: {account.GetBalance()}");
        }
        else {
            Console.WriteLine("Invalid account ID.");
        }
    }
}

// Пример использования
public class Program {
    public static void Main() {
        MainMenu menu = new MainMenu();
        menu.Start();
    }
}
