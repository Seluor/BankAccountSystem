using System;
using System.Collections.Generic;

public interface IAccountObservable {
    void Subscribe(IAccountObserver observer);
    void Unsubscribe(IAccountObserver observer);
    void Notify();
}

public interface IAccountObserver {
    void Update(Account account);
}

public class Account : IAccountObservable {
    private static int nextId = 1;
    private List<IAccountObserver> observers;
    private decimal balance;

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

public class User : IAccountObserver {
    private string name;

    public User(string name) {
        this.name = name;
    }

    public void Update(Account account) {
        Console.WriteLine($"Пользователь {name}: Баланс счета изменился и составляет {account.GetBalance()}");
    }

    public string GetName() {
        return name;
    }
}

public class MainMenu {
    private List<User> users;
    private List<Account> accounts;

    public MainMenu() {
        users = new List<User>();
        accounts = new List<Account>();
    }

    public void Start() {
        while (true) {
            Console.WriteLine("\n========== Банковская Система ==========");
            Console.WriteLine("1. Создать Пользователя");
            Console.WriteLine("2. Создать Счет");
            Console.WriteLine("3. Пополнить Счет");
            Console.WriteLine("4. Снять Деньги со Счета");
            Console.WriteLine("5. Перевести Деньги");
            Console.WriteLine("6. Проверить Баланс");
            Console.WriteLine("7. Выйти");
            Console.Write("Введите номер опции: ");

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
                    Console.WriteLine("Выход из программы...");
                    return;
                    default:
                    Console.WriteLine("Неверная опция. Пожалуйста, попробуйте снова.");
                    break;
                }
            }
            else {
                Console.WriteLine("Неверный ввод. Пожалуйста, попробуйте снова.");
            }
        }
    }

    private void CreateUser() {
        Console.Write("Введите имя пользователя: ");
        string name = Console.ReadLine();
        User user = new User(name);
        users.Add(user);
        Console.WriteLine($"Пользователь {name} успешно создан.");
    }

    private void CreateAccount() {
        if (users.Count == 0) {
            Console.WriteLine("Пользователи не найдены. Пожалуйста, сначала создайте пользователя.");
            return;
        }

        Console.Write("Введите имя пользователя: ");
        string name = Console.ReadLine();
        User user = users.Find(u => u.GetName() == name);

        if (user == null) {
            Console.WriteLine("Пользователь не найден.");
            return;
        }

        Console.Write("Введите начальный баланс: ");
        decimal balance;
        if (decimal.TryParse(Console.ReadLine(), out balance)) {
            Account account = new Account(balance);
            account.Subscribe(user);
            accounts.Add(account);
            Console.WriteLine($"Счет успешно создан. Идентификатор счета: {account.Id}");
        }
        else {
            Console.WriteLine("Неверная сумма баланса.");
        }
    }

    private void Deposit() {
        if (accounts.Count == 0) {
            Console.WriteLine("Счета не найдены. Пожалуйста, сначала создайте счет.");
            return;
        }

        Console.Write("Введите идентификатор счета: ");
        int accountId;
        if (int.TryParse(Console.ReadLine(), out accountId)) {
            Account account = accounts.Find(a => a.Id == accountId);

            if (account == null) {
                Console.WriteLine("Счет не найден.");
                return;
            }

            Console.Write("Введите сумму для пополнения: ");
            decimal amount;
            if (decimal.TryParse(Console.ReadLine(), out amount)) {
                account.Deposit(amount);
                Console.WriteLine("Пополнение счета прошло успешно.");
            }
            else {
                Console.WriteLine("Неверная сумма для пополнения.");
            }
        }
        else {
            Console.WriteLine("Неверный идентификатор счета.");
        }
    }

    private void Withdraw() {
        if (accounts.Count == 0) {
            Console.WriteLine("Счета не найдены. Пожалуйста, сначала создайте счет.");
            return;
        }

        Console.Write("Введите идентификатор счета: ");
        int accountId;
        if (int.TryParse(Console.ReadLine(), out accountId)) {
            Account account = accounts.Find(a => a.Id == accountId);

            if (account == null) {
                Console.WriteLine("Счет не найден.");
                return;
            }

            Console.Write("Введите сумму для снятия: ");
            decimal amount;
            if (decimal.TryParse(Console.ReadLine(), out amount)) {
                account.Withdraw(amount);
                Console.WriteLine("Снятие денег со счета прошло успешно.");
            }
            else {
                Console.WriteLine("Неверная сумма для снятия.");
            }
        }
        else {
            Console.WriteLine("Неверный идентификатор счета.");
        }
    }

    private void Transfer() {
        if (accounts.Count < 2) {
            Console.WriteLine("Для перевода необходимо как минимум два счета.");
            return;
        }

        Console.Write("Введите идентификатор исходного счета: ");
        int sourceAccountId;
        if (int.TryParse(Console.ReadLine(), out sourceAccountId)) {
            Account sourceAccount = accounts.Find(a => a.Id == sourceAccountId);

            if (sourceAccount == null) {
                Console.WriteLine("Исходный счет не найден.");
                return;
            }

            Console.Write("Введите идентификатор целевого счета: ");
            int targetAccountId;
            if (int.TryParse(Console.ReadLine(), out targetAccountId)) {
                Account targetAccount = accounts.Find(a => a.Id == targetAccountId);

                if (targetAccount == null) {
                    Console.WriteLine("Целевой счет не найден.");
                    return;
                }

                Console.Write("Введите сумму для перевода: ");
                decimal amount;
                if (decimal.TryParse(Console.ReadLine(), out amount)) {
                    sourceAccount.Transfer(amount, targetAccount);
                    Console.WriteLine("Перевод выполнен успешно.");
                }
                else {
                    Console.WriteLine("Неверная сумма для перевода.");
                }
            }
            else {
                Console.WriteLine("Неверный идентификатор целевого счета.");
            }
        }
        else {
            Console.WriteLine("Неверный идентификатор исходного счета.");
        }
    }

    private void CheckBalance() {
        if (accounts.Count == 0) {
            Console.WriteLine("Счета не найдены. Пожалуйста, сначала создайте счет.");
            return;
        }

        Console.Write("Введите идентификатор счета: ");
        int accountId;
        if (int.TryParse(Console.ReadLine(), out accountId)) {
            Account account = accounts.Find(a => a.Id == accountId);

            if (account == null) {
                Console.WriteLine("Счет не найден.");
                return;
            }

            Console.WriteLine($"Идентификатор счета: {account.Id}");
            Console.WriteLine($"Баланс счета: {account.GetBalance()}");
        }
        else {
            Console.WriteLine("Неверный идентификатор счета.");
        }
    }
}

public class Program {
    public static void Main() {
        MainMenu menu = new MainMenu();
        menu.Start();
    }
}
