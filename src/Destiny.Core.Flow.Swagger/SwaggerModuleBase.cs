﻿using Destiny.Core.Flow.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Destiny.Core.Flow.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Builder;
using Destiny.Core.Flow.Exceptions;
using Destiny.Core.Flow.Swagger.Filter;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Destiny.Core.Flow.Swagger
{
  public abstract  class SwaggerModuleBase: AppModuleBase
    {
        private string _url = string.Empty;
        private string _title = string.Empty;
        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            IConfiguration configuration=  services.GetConfiguration();
            var title= configuration["Destiny:Swagger:Title"];
            var version = configuration["Destiny:Swagger:Version"];
            var url = configuration["Destiny:Swagger:Url"];

            if (url.IsNullOrEmpty())
            {
                throw new AppException("Url不能为空 ！！！");
            }

            if (version.IsNullOrEmpty())
            {
                throw new AppException("版本号不能为空 ！！！");
            }

            if (title.IsNullOrEmpty())
            {

                throw new AppException("标题不能为空 ！！！");
            }
            _url = url;
            _title = title;
       
            services.AddSwaggerGen(s =>
            {

           
                s.SwaggerDoc(version, new OpenApiInfo { Title = title, Version = version });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;

                var files = Directory.GetFiles(basePath, "*.xml");
                foreach (var file in files)
                {
                    s.IncludeXmlComments(file, true);
                }
                //s.OperationFilter<AddResponseHeadersFilter>();
                //
                // Use method name as operationId
                s.CustomOperationIds(apiDesc =>
                {
                    return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
                });
                // TODO:一定要返回true！
                s.DocInclusionPredicate((docName, description) =>
                {
                    return true;
                });

                ////https://github.com/domaindrivendev/Swashbuckle.AspNetCore  
                //s.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                //s.OperationFilter<SecurityRequirementsOperationFilter>();  // 很重要！这里配置安全校验，和之前的版本不一样
                s.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",

                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {

                       new OpenApiSecurityScheme{
                         Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                       },
                       new[] { "readAccess", "writeAccess" }
                    }
                });

                //s.SchemaFilter<AutoRestSchemaFilter>();
                //s.DocumentFilter<TagDescriptionsDocumentFilter>();


            });
            return services;
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DefaultModelExpandDepth(2);
                c.DefaultModelRendering(ModelRendering.Example);
                c.DefaultModelsExpandDepth(-1);

                c.DisplayRequestDuration();
                c.DocExpansion(DocExpansion.None);
                c.EnableDeepLinking();
                c.EnableFilter();
                c.MaxDisplayedTags(5);
                c.ShowExtensions();
                c.EnableValidator();
    
                c.SwaggerEndpoint(_url, _title);
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
