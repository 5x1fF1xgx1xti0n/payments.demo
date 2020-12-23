using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payments.API.Hash;
using Payments.API.Models.Entities;
using Payments.API.Models.Entities.EncryptedModels;
using Payments.API.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Payments.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentsDbContext _context;
        private readonly IHasher _hasher;

        private const decimal COMMISSION_PERCENT = 0.02M;
        private const int ADMIN_ID = 1;
        private const int TOTAL_COMMISSION_ID = 1;
        private const decimal INITIAL_SUM = 1000;

        public PaymentsController(PaymentsDbContext paymentsDbContext, IHasher hasher)
        {
            _context = paymentsDbContext;
            _hasher = hasher;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(Credentials cred)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Login == cred.Login);
            if (user == null)
            {
                return BadRequest();
            }

            var hashedPassword = _hasher.Encrypt(cred.Password);

            if (user.PasswordHash != hashedPassword)
            {
                return BadRequest();
            }

            return Ok(new SessionData(user.Id, user.Role));
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpData data)
        {
            if (string.IsNullOrEmpty(data?.FullName) || string.IsNullOrEmpty(data?.Login) || string.IsNullOrEmpty(data?.Password))
            {
                return BadRequest();
            }

            var admin = await _context.Users.SingleOrDefaultAsync(x => x.Id == ADMIN_ID);

            var user = new User()
            {
                Login = data.Login,
                FullName = data.FullName,
                PasswordHash = _hasher.Encrypt(data.Password),
                Role = "User"
            };

            if (admin == null)
            {
                user.Role = "Admin";
            }

            var account = new Account()
            {
                Number = Guid.NewGuid().ToString(),
                EnctyptedSum = _hasher.Encrypt(INITIAL_SUM.ToString()),
                User = user
            };

            user.Account = account;

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new SessionData(user.Id, user.Role));
        }

        [HttpPost("commission")]
        public async Task<IActionResult> GetCommission(Sum sum)
        {
            if (sum.Value <= 0)
            {
                return BadRequest();
            }
            var commission = sum.Value * COMMISSION_PERCENT;
            var paymentWithCommission = new PaymentWithCommission(sum.Value, commission);
            return Ok(paymentWithCommission);
        }

        [HttpPost("transaction")]
        public async Task<IActionResult> CreateTransaction(NewTransactionData data)
        {
            if (data.RawSum <= 0)
            {
                return BadRequest();
            }

            var fromAcc = await _context.Accounts.SingleOrDefaultAsync(x => x.Number == data.FromAccountNumber);
            var toAcc = await _context.Accounts.SingleOrDefaultAsync(x => x.Number == data.ToAccountNumber);

            if (fromAcc == null || toAcc == null)
            {
                return BadRequest();
            }

            var td = new TransactionDetails()
            {
                Date = DateTime.Now,
                RawSum = data.RawSum,
                CommissionSum = data.RawSum * COMMISSION_PERCENT,
                TotalSum = data.RawSum * (COMMISSION_PERCENT + 1),
                FromAccountId = fromAcc.Id,
                ToAccountId = toAcc.Id
            };

            var fromDecryptedSum = Convert.ToDecimal(_hasher.Decrypt(fromAcc.EnctyptedSum));
            fromDecryptedSum -= td.TotalSum;
            fromAcc.EnctyptedSum = _hasher.Encrypt(fromDecryptedSum.ToString());

            var toDecryptedSum = Convert.ToDecimal(_hasher.Decrypt(toAcc.EnctyptedSum));
            toDecryptedSum += td.RawSum;
            toAcc.EnctyptedSum = _hasher.Encrypt(toDecryptedSum.ToString());

            var adminAcc = await _context.Accounts.SingleOrDefaultAsync(x => x.UserId == ADMIN_ID);
            var adminDecryptedSum = Convert.ToDecimal(_hasher.Decrypt(adminAcc.EnctyptedSum));
            adminDecryptedSum += td.CommissionSum;
            adminAcc.EnctyptedSum = _hasher.Encrypt(adminDecryptedSum.ToString());

            var transaction = new Transaction()
            {
                EncryptedTransactionDetails = _hasher.Encrypt(JsonSerializer.Serialize(td))
            };

            var totalComission = await _context.TotalCommissions.SingleOrDefaultAsync(x => x.Id == TOTAL_COMMISSION_ID);
            if (totalComission == null)
            {
                var encCommission = _hasher.Encrypt(td.CommissionSum.ToString());
                totalComission = new TotalCommission()
                {
                    EncryptedValue = encCommission
                };

                await _context.AddAsync(totalComission);
            }
            else
            {
                var dec = Convert.ToDecimal(_hasher.Decrypt(totalComission.EncryptedValue));
                dec += td.CommissionSum;
                totalComission.EncryptedValue = _hasher.Encrypt(dec.ToString());
                _context.Update(totalComission);
            }

            await _context.AddAsync(transaction);
            _context.Update(fromAcc);
            _context.Update(toAcc);
            _context.Update(adminAcc);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users
                .Include(x => x.Account)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return BadRequest();
            }

            var sum = Convert.ToDecimal(_hasher.Decrypt(user.Account.EnctyptedSum));

            var userVM = new UserViewModel()
            {
                Id = id,
                FullName = user.FullName,
                Login = user.Login,
                Role = user.Role,
                Account = new AccountViewModel()
                {
                    Id = user.Account.Id,
                    Number = user.Account.Number,
                    Sum = sum
                }
            };

            return Ok(userVM);
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _context.Transactions.ToListAsync();
            var transactionsVMs = new List<TransactionViewModel>(transactions.Count);

            foreach (var t in transactions)
            {
                var dec = JsonSerializer.Deserialize<TransactionDetails>(_hasher.Decrypt(t.EncryptedTransactionDetails));

                var fromAcc = await _context.Accounts.SingleOrDefaultAsync(x => x.Id == dec.FromAccountId);
                var toAcc = await _context.Accounts.SingleOrDefaultAsync(x => x.Id == dec.ToAccountId);

                if (fromAcc == null || toAcc == null)
                {
                    return BadRequest();
                }

                var tvm = new TransactionViewModel()
                {
                    Id = t.Id,
                    Date = dec.Date,
                    RawSum = dec.RawSum,
                    CommissionSum = dec.CommissionSum,
                    TotalSum = dec.TotalSum,
                    FromAccountNumber = fromAcc.Number,
                    ToAccountNumber = toAcc.Number
                };

                transactionsVMs.Add(tvm);
            }

            return Ok(transactionsVMs);
        }

        [HttpGet("transactions/{userId}")]
        public async Task<IActionResult> GetUserTransactions(int userId)
        {
            var user = await _context.Users.Include(x => x.Account).SingleOrDefaultAsync(x => x.Id == userId);
            var userAccNumber = user.Account.Number;
            var transactions = await _context.Transactions.ToListAsync();
            var transactionsVMs = new List<TransactionViewModel>(transactions.Count);

            foreach (var t in transactions)
            {
                var dec = JsonSerializer.Deserialize<TransactionDetails>(_hasher.Decrypt(t.EncryptedTransactionDetails));

                var fromAcc = await _context.Accounts.SingleOrDefaultAsync(x => x.Id == dec.FromAccountId);
                var toAcc = await _context.Accounts.SingleOrDefaultAsync(x => x.Id == dec.ToAccountId);

                if (fromAcc == null || toAcc == null)
                {
                    return BadRequest();
                }

                if (fromAcc.Number != userAccNumber && toAcc.Number != userAccNumber)
                {
                    continue;
                }

                var tvm = new TransactionViewModel()
                {
                    Id = t.Id,
                    Date = dec.Date,
                    RawSum = dec.RawSum,
                    CommissionSum = dec.CommissionSum,
                    TotalSum = dec.TotalSum,
                    FromAccountNumber = fromAcc.Number,
                    ToAccountNumber = toAcc.Number
                };

                transactionsVMs.Add(tvm);
            }

            return Ok(transactionsVMs);
        }

        [HttpGet("commission/total")]
        public async Task<IActionResult> GetTotalCommission()
        {
            var totalCommission = await _context.TotalCommissions.SingleOrDefaultAsync();
            if (totalCommission == null)
            {
                return StatusCode(500);
            }
            var decValue = Convert.ToDecimal(_hasher.Decrypt(totalCommission.EncryptedValue));
            return Ok(decValue);
        }
    }
}