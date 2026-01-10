// Updated BusinessLogicAuthDataSagaHandler for ApplicationUser-only implementation

using System;
using System.Threading;
using System.Threading.Tasks;

using PCIShield.Api.Auth.ApplicationUserOnly;
using PCIShield.Domain.Entities;
using PCIShield.Infrastructure.Services;

using PCIShieldLib.SharedKernel.Interfaces;

using LanguageExt;
using static LanguageExt.Prelude;

public class BusinessLogicAuthDataSagaHandler : IBusinessLogicAuthDataSagaHandler
{
    private readonly ValidateCredentialsCommand _validateCredentialsCommand;
    private readonly CreateIdentityUserCommand _createIdentityUserCommand;
    private readonly GenerateTenantIdCommand _generateTenantIdCommand;
    private readonly GenerateClaimsCommand _generateClaimsCommand;
    private readonly GenerateJwtTokenCommand _generateJwtTokenCommand;
    private readonly GenerateRefreshTokenCommand _generateRefreshTokenCommand;
    private readonly CreateApplicationUserCommand _createApplicationUserCommand;
    private readonly SendEmailConfirmationCommand _sendEmailConfirmationCommand;
    private readonly LoadUserDataForLoginCommand _loadUserDataForLoginCommand;
    private readonly UpdateUserStatusCommand _updateUserStatusCommand;
    private readonly PreLoadCacheCommand _preLoadCacheCommand;
    private readonly ConfirmEmailCommand _confirmEmailCommand;
    private readonly LogoutUserCommand _logoutUserCommand;
    private readonly RefreshTokenCommand _refreshTokenCommand;

    private readonly ForgotPasswordCommand _forgotPasswordCommand;
    private readonly ResetPasswordCommand _resetPasswordCommand;
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IAppLoggerService<BusinessLogicAuthDataSagaHandler> _logger;

    public BusinessLogicAuthDataSagaHandler(
        ValidateCredentialsCommand validateCredentialsCommand,
        CreateIdentityUserCommand createIdentityUserCommand,
        GenerateTenantIdCommand generateTenantIdCommand,
        GenerateClaimsCommand generateClaimsCommand,
        GenerateJwtTokenCommand generateJwtTokenCommand,
        GenerateRefreshTokenCommand generateRefreshTokenCommand,
        CreateApplicationUserCommand createApplicationUserCommand,
        SendEmailConfirmationCommand sendEmailConfirmationCommand,
        LoadUserDataForLoginCommand loadUserDataForLoginCommand,
        UpdateUserStatusCommand updateUserStatusCommand,
        PreLoadCacheCommand preLoadCacheCommand,
        ConfirmEmailCommand confirmEmailCommand,
        LogoutUserCommand logoutUserCommand,
        RefreshTokenCommand refreshTokenCommand,
            ForgotPasswordCommand forgotPasswordCommand,
    ResetPasswordCommand resetPasswordCommand,

        IRepository<ApplicationUser> userRepository,
        IAppLoggerService<BusinessLogicAuthDataSagaHandler> logger)
    {
        _validateCredentialsCommand = validateCredentialsCommand;
        _createIdentityUserCommand = createIdentityUserCommand;
        _generateTenantIdCommand = generateTenantIdCommand;
        _generateClaimsCommand = generateClaimsCommand;
        _generateJwtTokenCommand = generateJwtTokenCommand;
        _generateRefreshTokenCommand = generateRefreshTokenCommand;
        _createApplicationUserCommand = createApplicationUserCommand;
        _sendEmailConfirmationCommand = sendEmailConfirmationCommand;
        _loadUserDataForLoginCommand = loadUserDataForLoginCommand;
        _updateUserStatusCommand = updateUserStatusCommand;
        _preLoadCacheCommand = preLoadCacheCommand;
        _confirmEmailCommand = confirmEmailCommand;
        _logoutUserCommand = logoutUserCommand;
        _refreshTokenCommand = refreshTokenCommand;

        _forgotPasswordCommand = forgotPasswordCommand ?? throw new ArgumentNullException(nameof(forgotPasswordCommand));
        _resetPasswordCommand = resetPasswordCommand ?? throw new ArgumentNullException(nameof(resetPasswordCommand));

        _userRepository = userRepository;
        _logger = logger;
    }
    public async Task<Either<Exception, Unit>> ExecuteLoginAsync(LoginAuthContext context, CancellationToken cancellationToken)
    {
        try
        {
            _userRepository.BeginTransaction();
            var loginResult = await _validateCredentialsCommand
                .ExecuteAuthBusinessLogicAsync(context, cancellationToken)
                .BindAsync(async _ => await _loadUserDataForLoginCommand
                    .ExecuteAuthBusinessLogicAsync(context, cancellationToken))
                .BindAsync(async _ => await _generateRefreshTokenCommand
                    .ExecuteAuthBusinessLogicAsync(context, cancellationToken))
                .BindAsync(async _ => await _generateTenantIdCommand
                    .ExecuteAuthBusinessLogicAsync(context, cancellationToken))
                .BindAsync(async _ => await _generateClaimsCommand
                    .ExecuteAuthBusinessLogicAsync(context, cancellationToken))
                .BindAsync(async _ => await _generateJwtTokenCommand
                    .ExecuteAuthBusinessLogicAsync(context, cancellationToken))
                .BindAsync(async _ => await _updateUserStatusCommand
                    .ExecuteAuthBusinessLogicAsync(context, cancellationToken))
                .BindAsync(async _ => await _preLoadCacheCommand
                    .ExecuteAuthBusinessLogicAsync(context, cancellationToken));

            return loginResult.Match<Either<Exception, Unit>>(
                Right: _ =>
                {
                    _userRepository.CommitTransaction();
                    _logger.LogInformation(
                        "Login saga completed successfully for user {UserId} with tenant {TenantId}",
                        context.CurrentIdentityUser?.Id,
                        context.TenantId
                    );
                    return Right(unit);
                },
                Left: ex =>
                {
                    _userRepository.RollbackTransaction();
                    _logger.LogError(ex, "Login saga failed, transaction rolled back");
                    return Left(ex);
                }
            );
        }
        catch (Exception ex)
        {
            _userRepository.RollbackTransaction();
            _logger.LogError(ex, "Unexpected error in login saga");
            return Left(ex);
        }
    }
    public async Task<Either<Exception, Unit>> ExecuteRegisterAsync(RegisterAuthContext context, CancellationToken cancellationToken)
    {
        try
        {
            _userRepository.BeginTransaction();
            var registrationResult = await _createIdentityUserCommand
                .ExecuteAuthBusinessLogicAsync(context, cancellationToken)
                .BindAsync(async _ => await _generateTenantIdCommand.ExecuteAuthBusinessLogicAsync(context, cancellationToken))
                .BindAsync(async _ => await _createApplicationUserCommand.ExecuteAuthBusinessLogicAsync(context, cancellationToken))
                .BindAsync(async _ => await _sendEmailConfirmationCommand.ExecuteAuthBusinessLogicAsync(context, cancellationToken));

            return registrationResult.Match<Either<Exception, Unit>>(
                Right: _ =>
                {
                    _userRepository.CommitTransaction();
                    return Right(unit);
                },
                Left: ex =>
                {
                    _userRepository.RollbackTransaction();
                    return Left(ex);
                });
        }
        catch (Exception ex)
        {
            _userRepository.RollbackTransaction();
            return Left(ex);
        }
    }
    public async Task<Either<Exception, Unit>> ExecuteLogoutAsync(LogoutAuthContext context, CancellationToken cancellationToken)
    {
        try
        {
            _userRepository.BeginTransaction();
            var logoutResult = await _logoutUserCommand.ExecuteAuthBusinessLogicAsync(context, cancellationToken);

            return logoutResult.Match<Either<Exception, Unit>>(
                Right: _ =>
                {
                    _userRepository.CommitTransaction();
                    return Right(unit);
                },
                Left: ex =>
                {
                    _userRepository.RollbackTransaction();
                    return Left(ex);
                });
        }
        catch (Exception ex)
        {
            _userRepository.RollbackTransaction();
            return Left(ex);
        }
    }
    public async Task<Either<Exception, Unit>> ExecuteRefreshTokenAsync(RefreshTokenAuthContext context, CancellationToken cancellationToken)
    {
        try
        {
            _userRepository.BeginTransaction();
            var refreshResult = await _refreshTokenCommand.ExecuteAuthBusinessLogicAsync(context, cancellationToken);

            return refreshResult.Match<Either<Exception, Unit>>(
                Right: _ =>
                {
                    _userRepository.CommitTransaction();
                    return Right(unit);
                },
                Left: ex =>
                {
                    _userRepository.RollbackTransaction();
                    return Left(ex);
                });
        }
        catch (Exception ex)
        {
            _userRepository.RollbackTransaction();
            return Left(ex);
        }
    }
    public async Task<Either<Exception, Unit>> ExecuteConfirmEmailAsync(ConfirmEmailAuthContext context, CancellationToken cancellationToken)
    {
        try
        {
            _userRepository.BeginTransaction();
            var confirmationResult = await _confirmEmailCommand.ExecuteAuthBusinessLogicAsync(context, cancellationToken);

            return confirmationResult.Match<Either<Exception, Unit>>(
                Right: _ =>
                {
                    _userRepository.CommitTransaction();
                    return Right(unit);
                },
                Left: ex =>
                {
                    _userRepository.RollbackTransaction();
                    return Left(ex);
                });
        }
        catch (Exception ex)
        {
            _userRepository.RollbackTransaction();
            return Left(ex);
        }
    }

    public async Task<Either<Exception, Unit>> ExecuteForgotPasswordAsync(
    ForgotPasswordAuthContext context,
    CancellationToken cancellationToken)
    {
        try
        {

            _logger.LogInformation("Executing forgot password saga for email: {Email}", context.Email);

            var result = await _forgotPasswordCommand.ExecuteAuthBusinessLogicAsync(
                context,
                cancellationToken);

            return result.Match<Either<Exception, Unit>>(
                Right: _ =>
                {
                    _logger.LogInformation("Forgot password saga completed successfully");
                    return Right(unit);
                },
                Left: ex =>
                {
                    _logger.LogError(ex, "Forgot password saga failed");
                    return Left(ex);
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in forgot password saga");
            return Left(ex);
        }
    }

    public async Task<Either<Exception, Unit>> ExecuteResetPasswordAsync(
        ResetPasswordAuthContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            _userRepository.BeginTransaction();

            _logger.LogInformation("Executing reset password saga for email: {Email}", context.Email);

            var result = await _resetPasswordCommand.ExecuteAuthBusinessLogicAsync(
                context,
                cancellationToken);

            return result.Match<Either<Exception, Unit>>(
                Right: _ =>
                {
                    _userRepository.CommitTransaction();
                    _logger.LogInformation("Reset password saga completed successfully");
                    return Right(unit);
                },
                Left: ex =>
                {
                    _userRepository.RollbackTransaction();
                    _logger.LogError(ex, "Reset password saga failed, transaction rolled back");
                    return Left(ex);
                }
            );
        }
        catch (Exception ex)
        {
            _userRepository.RollbackTransaction();
            _logger.LogError(ex, "Unexpected error in reset password saga");
            return Left(ex);
        }
    }

}