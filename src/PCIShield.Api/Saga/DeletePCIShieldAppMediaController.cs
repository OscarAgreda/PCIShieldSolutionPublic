using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using BlazorMauiShared.Models.Merchant;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;

using PCIShield.Api.Auth;

using PCIShield.Domain.Entities;
using PCIShield.Domain.Specifications;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;

using Serilog.Parsing;
using NRedisStack.Search;

namespace PCIShield.Api.Media;
public class DeletePCIShieldAppMediaControllerSoaps 
{
    private readonly PCIShieldLib.SharedKernel.Interfaces.IRepository<ComplianceOfficer> _complianceOfficerDbRepository;
    private readonly IConfiguration _configuration;
    private readonly PCIShieldLib.SharedKernel.Interfaces.IRepository<Merchant> _merchantDbRepository;
    private readonly IMailKitEmailServicePlus _emailService;
    private readonly IGridFsBlobStorageService _gridFsBlobStorageService;
    private readonly IAppLoggerService<PCIShieldAppMediaController> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly PCIShieldLib.SharedKernel.Interfaces.IRepository<ApplicationUser> _userDbRepository;
    private readonly UserManager<CustomPCIShieldUser> _userManager;
}

