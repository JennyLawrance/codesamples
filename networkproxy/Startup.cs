using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace networkproxy
{
    public class Startup
    {
        HttpClient httpClient = new HttpClient();
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/proxy", async context =>
                {
                    string query = string.Empty;
                    if (context.Request.Query.ContainsKey("url"))
                    {
                        try
                        {
                            string url = "";
                            bool isIpPresent = false;
                            query = context.Request.Query["url"];
                            if (context.Request.Query.ContainsKey("ip"))
                            {
                                url = $"http://{context.Request.Query["ip"]}/";
                                isIpPresent = true;
                            }
                            else
                            {
                                url = query;
                            }

                            HttpRequestMessage rq = new HttpRequestMessage(HttpMethod.Get, url);
                            if (isIpPresent)
                            {
                                rq.Headers.Host = query;
                            }
                            HttpResponseMessage response = await httpClient.SendAsync(rq);
                            string responseContent = await response.Content.ReadAsStringAsync();
                            await context.Response.WriteAsync($"response from {query}: {responseContent}");
                        }
                        catch (Exception e)
                        {
                            await context.Response.WriteAsync($"Exception {e.ToString()}");
                        }
                    }
                });

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
