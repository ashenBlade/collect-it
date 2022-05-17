using Microsoft.AspNetCore.Identity;

namespace CollectIt.MVC.Infrastructure.Account;

public class RussianLanguageIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError() {Description = "В пароле должны содержаться цифры"};
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError() {Description = "Пользователь с такой почтой уже зарегистрирован"};
    }

    public override IdentityError PasswordMismatch()
    {
        return new IdentityError() {Description = "Пароли не совпадают"};
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError() {Description = "Пользователь с таким именем уже существует"};
    }

    public override IdentityError InvalidEmail(string email)
    {
        return new IdentityError() {Description = "Почта в неправильном формате"};
    }

    public override IdentityError InvalidUserName(string userName)
    {
        return new IdentityError() {Description = "Неправильное имя пользователя"};
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError() {Description = $"Минимальная длина пароля - {length} символов"};
    }

    public override IdentityError ConcurrencyFailure()
    {
        return new IdentityError() {Description = "Ошибка одновременного обновления"};
    }
}