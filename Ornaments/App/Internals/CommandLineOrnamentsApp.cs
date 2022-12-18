using Microsoft.Extensions.DependencyInjection;
using Ornaments.Extensions;
using Ornaments.App.Internals.Net;
using Ornaments.Solutions;
using Spectre.Console;
using System.CommandLine;
using System.CommandLine.Parsing;
using Ornaments.Data;
using Microsoft.EntityFrameworkCore;

namespace Ornaments.App.Internals;

internal class CommandLineOrnamentsApp : IOrnamentsApp
{
    private IServiceProvider serviceProvider;
    private RootCommand rootCommand;

    public CommandLineOrnamentsApp(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;

        var dbContext = serviceProvider.GetRequiredService<OrnamentsContext>();
        dbContext.Database.Migrate();

        AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Default)
            .Start("Configuring Ornaments", ctx =>
            {
                var year = new Option<int>(new[] { "-y", "--year" }, () => 2022, "Advent of Code event year.");
                var day = new Option<int>(new[] { "-d", "--day" }, () => 1, "Advent of Code event date.");
                var dryRun = new Option<bool>("--dry-run", () => false, "Disable automatic submission of answers.");
                var timeout = new Option<int>(new[] { "-t", "--timeout" }, () => 15, "Maximum execution time in seconds. Note: All challenges have a solution taking at most 15s to run.");
                var tokenTypes = new Option<IEnumerable<TokenType>>(new[] { "-u", "--users" }, () => new[] { TokenType.GitHub }, "Specifies which user tokens to submit answers for.");
                var years = new Option<IEnumerable<int>>(new[] { "-y", "--year" }, () => Enumerable.Range(2015, DateTime.Now.Year - 2015 + 1), "Advent of Code event year.");
                var runs = new Option<int>(new[] { "-r", "--runs" }, () => 1000, "Number of runs to average execution time over.");

                year.AddValidator(result =>
                {
                    var value = result.GetValueOrDefault<int>();
                    if (value.IsNotInRange(2015, DateTime.Now.Year))
                    {
                        result.ErrorMessage = $"Value must be between 2015 and {DateTime.Now.Year} (inclusive.)";
                    }
                });
                day.AddValidator(result =>
                {
                    var value = result.GetValueOrDefault<int>();
                    if (value.IsNotInRange(1, 25))
                    {
                        result.ErrorMessage = "Value must be between 1 and 25 (inclusive.)";
                    }
                });
                timeout.AddValidator(result =>
                {
                    var value = result.GetValueOrDefault<int>();
                    if (value.IsNotInRange(15, 300))
                    {
                        result.ErrorMessage = "Value must be between 15 and 300 (inclusive.)";
                    }
                });
                years.AddValidator(result =>
                {
                    var value = result.GetValueOrDefault<IEnumerable<int>>() ?? Enumerable.Empty<int>();
                    if (value.Any(x => x.IsNotInRange(2015, DateTime.Now.Year)))
                    {
                        result.ErrorMessage = $"Value must be between 2015 and {DateTime.Now.Year} (inclusive.)";
                    }
                });
                runs.AddValidator(result =>
                {
                    var value = result.GetValueOrDefault<int>();
                    if (value.IsNotInRange(1000, 1000000))
                    {
                        result.ErrorMessage = $"Value must be between 10 and 1000 (inclusive.)";
                    }
                });

                var solveCommand = new Command("solve", "Run Advent of Code event solutions.") { year, day, dryRun, tokenTypes, timeout };
                var solveCommandArgumentsBinder = new SolveCommandArgumentsBinder(year, day, dryRun, tokenTypes, timeout);
                solveCommand.SetHandler(HandleSolveCommandAsync, solveCommandArgumentsBinder);

                var listCommand = new Command("list", "List available Advent of Code event solutions.") { years };
                var listCommandArgumentsBinder = new ListCommandArgumentsBinder(years);
                listCommand.SetHandler(HandleListCommandAsync, listCommandArgumentsBinder);

                var benchmarkCommand = new Command("benchmark", "Benchmark a solution.") { year, day, runs };
                var benchmarkCommandArgumentsBinder = new BenchmarkCommandArgumentsBinder(year, day, runs);
                benchmarkCommand.SetHandler(HandleBenchmarkCommandAsync, benchmarkCommandArgumentsBinder);

                rootCommand = new RootCommand("Ornament — An Advent of Code SDK.");
                rootCommand.AddCommand(solveCommand);
                rootCommand.AddCommand(listCommand);
                rootCommand.AddCommand(benchmarkCommand);
            });

        ArgumentNullException.ThrowIfNull(rootCommand);
    }

    public async Task RunAsync(string[] args)
    {
        await rootCommand.InvokeAsync(args);
    }

    private async Task HandleSolveCommandAsync(SolveCommandArguments solveCommandArguments)
    {
        var solutionDescriptors = serviceProvider.GetRequiredService<IEnumerable<SolutionDescriptor>>();
        if (solutionDescriptors.IsEmpty())
        {
            AnsiConsole.WriteLine("There are no solutions registered.");
            return;
        }

        var descriptor = solutionDescriptors.SingleOrDefault(x => x.Attributes.Year == solveCommandArguments.Year && x.Attributes.Day == solveCommandArguments.Day);
        if (descriptor is null)
        {
            AnsiConsole.WriteLine($"There are no solutions registered matching event year {solveCommandArguments.Year}, day {solveCommandArguments.Day}.");
            return;
        }

        await AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Default)
            .StartAsync($"[yellow]Solving {descriptor.Attributes.Day}/{descriptor.Attributes.Year}[/]", async ctx =>
            {
                AnsiConsole.MarkupLine($"Event Year [green][bold]{descriptor.Attributes.Year}[/][/], Day [green][bold]{descriptor.Attributes.Day}[/][/]: {descriptor.Attributes.Name}");
                ctx.Status($"[yellow]Solving {descriptor.Attributes.Day}/{descriptor.Attributes.Year} -- fetching input from server.[/]");
                var client = serviceProvider.GetRequiredService<AocRestClient>();
                var input = await client.GetInputAsync(solveCommandArguments.TokenTypes.FirstOrDefault(), solveCommandArguments.Year, solveCommandArguments.Day);

                ctx.Status($"[yellow]Solving {descriptor.Attributes.Day}/{descriptor.Attributes.Year} -- configuring.[/]");
                var context = Context.Create(descriptor.Instance, input.Value);

                ctx.Status($"[yellow]Solving {descriptor.Attributes.Day}/{descriptor.Attributes.Year} -- running part A.[/]");
                var partA = await descriptor.Instance.DoPartOneAsync(context);
                AnsiConsole.MarkupLine($"Part A: [green][bold]{partA}[/][/]");

                ctx.Status($"[yellow]Solving {descriptor.Attributes.Day}/{descriptor.Attributes.Year} -- running part B.[/]");
                var partB = await descriptor.Instance.DoPartTwoAsync(context);
                AnsiConsole.MarkupLine($"Part B: [green][bold]{partB}[/][/]");
            });
    }

    private void HandleListCommandAsync(ListCommandArguments listCommandArguments)
    {
        var solutionDescriptors = serviceProvider.GetRequiredService<IEnumerable<SolutionDescriptor>>();
        if (solutionDescriptors.IsEmpty())
        {
            AnsiConsole.MarkupLine("There are no solutions registered.");
            return;
        }

        foreach (var year in listCommandArguments.Years)
        {
            var descriptorsForYear = solutionDescriptors.Where(x => x.Attributes.Year == year);
            if (descriptorsForYear.Any())
            {
                var table = new Table();
                table.Centered();
                table.Expand();
                table.Title($"Event Year [green][bold]{year}[/][/]");
                table.Border(TableBorder.Rounded);
                table.BorderColor(Color.Grey);
                table.AddColumn(new TableColumn("Date").LeftAligned());
                table.AddColumn(new TableColumn("Name").LeftAligned());
                foreach (var descriptor in descriptorsForYear)
                {
                    table.AddRow(descriptor.Attributes.Day.ToString(), descriptor.Attributes.Name);
                }
                AnsiConsole.Write(table);
            }
        }
    }

    private async Task HandleBenchmarkCommandAsync(BenchmarkCommandArguments benchmarkCommandArguments)
    {
        var solutionDescriptors = serviceProvider.GetRequiredService<IEnumerable<SolutionDescriptor>>();
        if (solutionDescriptors.IsEmpty())
        {
            AnsiConsole.WriteLine("There are no solutions registered.");
            return;
        }

        var descriptor = solutionDescriptors.SingleOrDefault(x => x.Attributes.Year == benchmarkCommandArguments.Year && x.Attributes.Day == benchmarkCommandArguments.Day);
        if (descriptor is null)
        {
            AnsiConsole.WriteLine($"There are no solutions registered matching event year {benchmarkCommandArguments.Year}, day {benchmarkCommandArguments.Day}.");
            return;
        }

        await AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Default)
            .StartAsync($"[yellow]Solving {descriptor.Attributes.Day}/{descriptor.Attributes.Year}[/]", async ctx =>
            {
                AnsiConsole.MarkupLine($"Event Year [green][bold]{descriptor.Attributes.Year}[/][/], Day [green][bold]{descriptor.Attributes.Day}[/][/]: {descriptor.Attributes.Name}");
                ctx.Status($"[yellow]Benchmarking {descriptor.Attributes.Day}/{descriptor.Attributes.Year} -- configuring.[/]");
                var context = Context.Create(descriptor.Instance, "18313\r\n2404\r\n10479\r\n\r\n7011\r\n10279\r\n1496\r\n10342\r\n8918\r\n3162\r\n4525\r\n4368\r\n\r\n17242\r\n\r\n10920\r\n14072\r\n9754\r\n4435\r\n9396\r\n\r\n5915\r\n2602\r\n4032\r\n3303\r\n2685\r\n1856\r\n1334\r\n4865\r\n6385\r\n1733\r\n5328\r\n\r\n8899\r\n5482\r\n3195\r\n7837\r\n8986\r\n13794\r\n\r\n25121\r\n22211\r\n21257\r\n\r\n6360\r\n4007\r\n5124\r\n2266\r\n6943\r\n6966\r\n3887\r\n3427\r\n1255\r\n5266\r\n6119\r\n2841\r\n\r\n6167\r\n3883\r\n2776\r\n5894\r\n2013\r\n6930\r\n6613\r\n4637\r\n5259\r\n2347\r\n3550\r\n5639\r\n5117\r\n\r\n4530\r\n5961\r\n5956\r\n2503\r\n5060\r\n4770\r\n5240\r\n4123\r\n4089\r\n6794\r\n3333\r\n5244\r\n4415\r\n\r\n1733\r\n1209\r\n4458\r\n1223\r\n3859\r\n7728\r\n9793\r\n8350\r\n\r\n2278\r\n3241\r\n4668\r\n1836\r\n3669\r\n2996\r\n1772\r\n5016\r\n6294\r\n6367\r\n1080\r\n5964\r\n5547\r\n\r\n4793\r\n13744\r\n9478\r\n7979\r\n1490\r\n12490\r\n\r\n6770\r\n7282\r\n2059\r\n5242\r\n13237\r\n9365\r\n\r\n2230\r\n6498\r\n1352\r\n3846\r\n3863\r\n6205\r\n5803\r\n2646\r\n4188\r\n4431\r\n6380\r\n1526\r\n2620\r\n4438\r\n\r\n24331\r\n23578\r\n\r\n4199\r\n5126\r\n5725\r\n1123\r\n2415\r\n5427\r\n5789\r\n4856\r\n2610\r\n4833\r\n3398\r\n4239\r\n1432\r\n5122\r\n\r\n5013\r\n4939\r\n11760\r\n8419\r\n5706\r\n3765\r\n10182\r\n\r\n11795\r\n9688\r\n1712\r\n2091\r\n9651\r\n13022\r\n\r\n10393\r\n11862\r\n1672\r\n5431\r\n6812\r\n1866\r\n5599\r\n\r\n2036\r\n6561\r\n4097\r\n3467\r\n4739\r\n7656\r\n3252\r\n4322\r\n5068\r\n2345\r\n4155\r\n\r\n3090\r\n1228\r\n1270\r\n8407\r\n8118\r\n4996\r\n7157\r\n3944\r\n6230\r\n2728\r\n\r\n5669\r\n2113\r\n3076\r\n1290\r\n7673\r\n1022\r\n5805\r\n5128\r\n7912\r\n7862\r\n4955\r\n\r\n20640\r\n\r\n4131\r\n4874\r\n5220\r\n9947\r\n9739\r\n1662\r\n9418\r\n9684\r\n\r\n4875\r\n3073\r\n1151\r\n11911\r\n10310\r\n10793\r\n10307\r\n\r\n6979\r\n4265\r\n8322\r\n3518\r\n1855\r\n4912\r\n5481\r\n7124\r\n4664\r\n\r\n19975\r\n16112\r\n19450\r\n7231\r\n\r\n3909\r\n4743\r\n3794\r\n4448\r\n3200\r\n3447\r\n3331\r\n7011\r\n3345\r\n3935\r\n6750\r\n\r\n3900\r\n14850\r\n4890\r\n9625\r\n4832\r\n\r\n23682\r\n1375\r\n\r\n7463\r\n8112\r\n10624\r\n1105\r\n6625\r\n5157\r\n5839\r\n1019\r\n\r\n19089\r\n36719\r\n\r\n5372\r\n7085\r\n8536\r\n8717\r\n6255\r\n5961\r\n3198\r\n7729\r\n3055\r\n\r\n4406\r\n2263\r\n6686\r\n1458\r\n2377\r\n2740\r\n1284\r\n3045\r\n7121\r\n3070\r\n\r\n19161\r\n10515\r\n1027\r\n\r\n7782\r\n1703\r\n1864\r\n3611\r\n1039\r\n2394\r\n3472\r\n6964\r\n5824\r\n8011\r\n3341\r\n\r\n56690\r\n\r\n5107\r\n1876\r\n4973\r\n5485\r\n4325\r\n6079\r\n3187\r\n3565\r\n2875\r\n7137\r\n3887\r\n6763\r\n\r\n2633\r\n2151\r\n1105\r\n2400\r\n5338\r\n3382\r\n1330\r\n3945\r\n6556\r\n1670\r\n5526\r\n4281\r\n2168\r\n\r\n5382\r\n2347\r\n4225\r\n1905\r\n4672\r\n6706\r\n5747\r\n5808\r\n1159\r\n4878\r\n5923\r\n\r\n11148\r\n8722\r\n3896\r\n15697\r\n3499\r\n\r\n46749\r\n\r\n13315\r\n20966\r\n\r\n1625\r\n6338\r\n7882\r\n1505\r\n4226\r\n9948\r\n6438\r\n6012\r\n\r\n1003\r\n10033\r\n5066\r\n7756\r\n10151\r\n10719\r\n5299\r\n5629\r\n\r\n4466\r\n6290\r\n7220\r\n3657\r\n8261\r\n7348\r\n8525\r\n4531\r\n4892\r\n7833\r\n\r\n24723\r\n7817\r\n\r\n4234\r\n4428\r\n5810\r\n2091\r\n4301\r\n1378\r\n2342\r\n3872\r\n1167\r\n2073\r\n6867\r\n7026\r\n\r\n7005\r\n10018\r\n3954\r\n4497\r\n4951\r\n1283\r\n5383\r\n\r\n4152\r\n4390\r\n8731\r\n8521\r\n5415\r\n1029\r\n8200\r\n3769\r\n\r\n7224\r\n12281\r\n8337\r\n8969\r\n13938\r\n9651\r\n\r\n10923\r\n14358\r\n8896\r\n8401\r\n7073\r\n\r\n3934\r\n4424\r\n4538\r\n4371\r\n2335\r\n6363\r\n1264\r\n4331\r\n5107\r\n2270\r\n3109\r\n2547\r\n2304\r\n5911\r\n\r\n4600\r\n5674\r\n2620\r\n1846\r\n3804\r\n2387\r\n4094\r\n3648\r\n4241\r\n5593\r\n2974\r\n2101\r\n1621\r\n3939\r\n\r\n7429\r\n9425\r\n9653\r\n5835\r\n\r\n13414\r\n4517\r\n15783\r\n3867\r\n\r\n2287\r\n1019\r\n11604\r\n11910\r\n2591\r\n2468\r\n10543\r\n\r\n13050\r\n1396\r\n13959\r\n6883\r\n\r\n6303\r\n2889\r\n4047\r\n3954\r\n2245\r\n6672\r\n4574\r\n5731\r\n5879\r\n2837\r\n5859\r\n4812\r\n6785\r\n\r\n10290\r\n10131\r\n5605\r\n3321\r\n2122\r\n6331\r\n9107\r\n7937\r\n\r\n1786\r\n1362\r\n7409\r\n2093\r\n3901\r\n1306\r\n2089\r\n1558\r\n4163\r\n6118\r\n6270\r\n1929\r\n\r\n4224\r\n3864\r\n3922\r\n3609\r\n2496\r\n2246\r\n5397\r\n1535\r\n5499\r\n4932\r\n3684\r\n1348\r\n3437\r\n3665\r\n\r\n2973\r\n4506\r\n3971\r\n2511\r\n4563\r\n3168\r\n2687\r\n6249\r\n2593\r\n6470\r\n4100\r\n6309\r\n1316\r\n4362\r\n\r\n14966\r\n2940\r\n11485\r\n\r\n6675\r\n16873\r\n5695\r\n14706\r\n\r\n3953\r\n\r\n5620\r\n1678\r\n3630\r\n5015\r\n3277\r\n4957\r\n5288\r\n5555\r\n2860\r\n1750\r\n2795\r\n6170\r\n6308\r\n\r\n3671\r\n1354\r\n4018\r\n4730\r\n4070\r\n1685\r\n4478\r\n1918\r\n5532\r\n3970\r\n3751\r\n4890\r\n4277\r\n2737\r\n5535\r\n\r\n14799\r\n8129\r\n\r\n5254\r\n2065\r\n1282\r\n2407\r\n5174\r\n4549\r\n4326\r\n1201\r\n3372\r\n3906\r\n6234\r\n4937\r\n4234\r\n2149\r\n\r\n5194\r\n2519\r\n3555\r\n6929\r\n4367\r\n4547\r\n\r\n1024\r\n37240\r\n\r\n3693\r\n6526\r\n2679\r\n5434\r\n7924\r\n2974\r\n1754\r\n4287\r\n4744\r\n4187\r\n1348\r\n\r\n6598\r\n5963\r\n6035\r\n5404\r\n3270\r\n1250\r\n5947\r\n3037\r\n3927\r\n3702\r\n5255\r\n1640\r\n6476\r\n\r\n4499\r\n5207\r\n1991\r\n4613\r\n3088\r\n5024\r\n4737\r\n5160\r\n2450\r\n3582\r\n2605\r\n2573\r\n1100\r\n3732\r\n\r\n1633\r\n5663\r\n2415\r\n4039\r\n4957\r\n2697\r\n5428\r\n5050\r\n3979\r\n3428\r\n4563\r\n3770\r\n3635\r\n4012\r\n4773\r\n\r\n7377\r\n6189\r\n10812\r\n6905\r\n11196\r\n5394\r\n10135\r\n\r\n10008\r\n2868\r\n2630\r\n8542\r\n8897\r\n2718\r\n5831\r\n4460\r\n\r\n1303\r\n4771\r\n1574\r\n2960\r\n5070\r\n1596\r\n5530\r\n4177\r\n4210\r\n6465\r\n4179\r\n1000\r\n4460\r\n6173\r\n\r\n6654\r\n1378\r\n2148\r\n8704\r\n6875\r\n4511\r\n7540\r\n6078\r\n5585\r\n4030\r\n\r\n4333\r\n5953\r\n5095\r\n4782\r\n3651\r\n4673\r\n3267\r\n6389\r\n2358\r\n2120\r\n3684\r\n3034\r\n3420\r\n1322\r\n\r\n8922\r\n9535\r\n16357\r\n16552\r\n\r\n1378\r\n1039\r\n1262\r\n2067\r\n6491\r\n2776\r\n1352\r\n4914\r\n5832\r\n1369\r\n6105\r\n1384\r\n2474\r\n1704\r\n\r\n6256\r\n2538\r\n4701\r\n4090\r\n4077\r\n5333\r\n2777\r\n4082\r\n4640\r\n2542\r\n6356\r\n3065\r\n4855\r\n2535\r\n\r\n6510\r\n3709\r\n4197\r\n4344\r\n5906\r\n6852\r\n7398\r\n3578\r\n3308\r\n1239\r\n3817\r\n5676\r\n\r\n5441\r\n3368\r\n1752\r\n2629\r\n1923\r\n4711\r\n1667\r\n3701\r\n1350\r\n4764\r\n6439\r\n2341\r\n4052\r\n\r\n6407\r\n5327\r\n1229\r\n4834\r\n6008\r\n1302\r\n5595\r\n5260\r\n3680\r\n3220\r\n3154\r\n3353\r\n1942\r\n1740\r\n\r\n2356\r\n1908\r\n3058\r\n4668\r\n3641\r\n1663\r\n4199\r\n2190\r\n6067\r\n3923\r\n3288\r\n1978\r\n6110\r\n5330\r\n\r\n7020\r\n2487\r\n6521\r\n1084\r\n6582\r\n1784\r\n7861\r\n2759\r\n8077\r\n2961\r\n\r\n68314\r\n\r\n2160\r\n15170\r\n2267\r\n3867\r\n6851\r\n\r\n8543\r\n4118\r\n22260\r\n\r\n5313\r\n3525\r\n1483\r\n3613\r\n5798\r\n1472\r\n4020\r\n5607\r\n4391\r\n2502\r\n5512\r\n5535\r\n3755\r\n5590\r\n3668\r\n\r\n25502\r\n7623\r\n15508\r\n\r\n2138\r\n1227\r\n11355\r\n1288\r\n7062\r\n5391\r\n3994\r\n\r\n7738\r\n3667\r\n5000\r\n1691\r\n7322\r\n8054\r\n4540\r\n2353\r\n6870\r\n8475\r\n\r\n4289\r\n6415\r\n1251\r\n4933\r\n7224\r\n2608\r\n2320\r\n4432\r\n4637\r\n1395\r\n6575\r\n5581\r\n\r\n6841\r\n1161\r\n5617\r\n12505\r\n6274\r\n9171\r\n\r\n13548\r\n20154\r\n20682\r\n\r\n2843\r\n7363\r\n5967\r\n15256\r\n16032\r\n\r\n7940\r\n6992\r\n3940\r\n3100\r\n1334\r\n1389\r\n5182\r\n6666\r\n4030\r\n6116\r\n3036\r\n\r\n35234\r\n19273\r\n\r\n7268\r\n10689\r\n\r\n1846\r\n4211\r\n1504\r\n3780\r\n3789\r\n2017\r\n6279\r\n5001\r\n2341\r\n6350\r\n5169\r\n1966\r\n2640\r\n5574\r\n\r\n2125\r\n8432\r\n8339\r\n8130\r\n3506\r\n6115\r\n4684\r\n9548\r\n3300\r\n\r\n6848\r\n1274\r\n6463\r\n12007\r\n10719\r\n10590\r\n5616\r\n\r\n4471\r\n1304\r\n1635\r\n5960\r\n3702\r\n1747\r\n3658\r\n2446\r\n4724\r\n5675\r\n1739\r\n3170\r\n3556\r\n\r\n9134\r\n12963\r\n6987\r\n\r\n5728\r\n7877\r\n6499\r\n5909\r\n1119\r\n3090\r\n5033\r\n4884\r\n3151\r\n\r\n10675\r\n8053\r\n9237\r\n14206\r\n11710\r\n\r\n49154\r\n\r\n5999\r\n3861\r\n1836\r\n3451\r\n3673\r\n5193\r\n2680\r\n6068\r\n1743\r\n5600\r\n4793\r\n6211\r\n2967\r\n1021\r\n\r\n24720\r\n\r\n8749\r\n11674\r\n4039\r\n5341\r\n14256\r\n\r\n4553\r\n3893\r\n1528\r\n5490\r\n2417\r\n2392\r\n4023\r\n2430\r\n4649\r\n3163\r\n3673\r\n1378\r\n1957\r\n1055\r\n\r\n10777\r\n4742\r\n16291\r\n16430\r\n12295\r\n\r\n6502\r\n6052\r\n2944\r\n8166\r\n7567\r\n3580\r\n5957\r\n4691\r\n1623\r\n3269\r\n\r\n1013\r\n5809\r\n16958\r\n4994\r\n\r\n36633\r\n\r\n8535\r\n13662\r\n24448\r\n\r\n6651\r\n7911\r\n\r\n19793\r\n2436\r\n\r\n4671\r\n4420\r\n5322\r\n1215\r\n4952\r\n1193\r\n2387\r\n2799\r\n2748\r\n1343\r\n4123\r\n1715\r\n2978\r\n2148\r\n6050\r\n\r\n2036\r\n4670\r\n5444\r\n4584\r\n4220\r\n6061\r\n2834\r\n2319\r\n1370\r\n4572\r\n4249\r\n4144\r\n3236\r\n3812\r\n3487\r\n\r\n55521\r\n\r\n3845\r\n1473\r\n6794\r\n4597\r\n5566\r\n2735\r\n2660\r\n6402\r\n2515\r\n3439\r\n\r\n3772\r\n12809\r\n5790\r\n10817\r\n9729\r\n\r\n1788\r\n2476\r\n4333\r\n2940\r\n7232\r\n3582\r\n8604\r\n3742\r\n7644\r\n4823\r\n\r\n6128\r\n6737\r\n5712\r\n4168\r\n4465\r\n2183\r\n1217\r\n2257\r\n6326\r\n5966\r\n5699\r\n2985\r\n2290\r\n\r\n5190\r\n4155\r\n10754\r\n9830\r\n4507\r\n2165\r\n7208\r\n4409\r\n\r\n4256\r\n4158\r\n1941\r\n3124\r\n5852\r\n3376\r\n6411\r\n5508\r\n5072\r\n2374\r\n4189\r\n4032\r\n4957\r\n2050\r\n\r\n2737\r\n6772\r\n4468\r\n2072\r\n1214\r\n6231\r\n7077\r\n6100\r\n6990\r\n5676\r\n\r\n7474\r\n4524\r\n8778\r\n7129\r\n8151\r\n7975\r\n7277\r\n8672\r\n8097\r\n3847\r\n\r\n29172\r\n7125\r\n\r\n50466\r\n\r\n14169\r\n22037\r\n\r\n4483\r\n1801\r\n4863\r\n1920\r\n2776\r\n1407\r\n3998\r\n1142\r\n2664\r\n3920\r\n5708\r\n5481\r\n5701\r\n5159\r\n3654\r\n\r\n1960\r\n4419\r\n5933\r\n4941\r\n4511\r\n1120\r\n5179\r\n3447\r\n5285\r\n3675\r\n1333\r\n1418\r\n4595\r\n4140\r\n2353\r\n\r\n1995\r\n5588\r\n1443\r\n5681\r\n3439\r\n6178\r\n5795\r\n5144\r\n2788\r\n6232\r\n2620\r\n4665\r\n5624\r\n\r\n37076\r\n7362\r\n\r\n3885\r\n8268\r\n10947\r\n9084\r\n3016\r\n10383\r\n9594\r\n\r\n16223\r\n16281\r\n5352\r\n11764\r\n10192\r\n\r\n5248\r\n3151\r\n7341\r\n8125\r\n10671\r\n13510\r\n\r\n1759\r\n7635\r\n7021\r\n3620\r\n7640\r\n4967\r\n3662\r\n3840\r\n7613\r\n3948\r\n1820\r\n\r\n4010\r\n3936\r\n7375\r\n8060\r\n12396\r\n8236\r\n\r\n5033\r\n2739\r\n4296\r\n5564\r\n6502\r\n3501\r\n5637\r\n5826\r\n5862\r\n5579\r\n6416\r\n2277\r\n2471\r\n\r\n4325\r\n2621\r\n1600\r\n4290\r\n15494\r\n\r\n6834\r\n15459\r\n10418\r\n12560\r\n6382\r\n\r\n8598\r\n1089\r\n11386\r\n9538\r\n6933\r\n3943\r\n\r\n14268\r\n7172\r\n7680\r\n15936\r\n\r\n5695\r\n\r\n69893\r\n\r\n7293\r\n4968\r\n7862\r\n3210\r\n1924\r\n4742\r\n3519\r\n1095\r\n5922\r\n6335\r\n2405\r\n\r\n4233\r\n3819\r\n7282\r\n10018\r\n3820\r\n5380\r\n9377\r\n8889\r\n\r\n13183\r\n9964\r\n4473\r\n12469\r\n9862\r\n6567\r\n\r\n4903\r\n1198\r\n5238\r\n1582\r\n1820\r\n6080\r\n10178\r\n\r\n4554\r\n6315\r\n2818\r\n5956\r\n2491\r\n5291\r\n5056\r\n3699\r\n2843\r\n1243\r\n5430\r\n2175\r\n\r\n31126\r\n\r\n5392\r\n1590\r\n3738\r\n2507\r\n2817\r\n6963\r\n6140\r\n4606\r\n2814\r\n7044\r\n1282\r\n\r\n12230\r\n1430\r\n4283\r\n11385\r\n2557\r\n6868\r\n\r\n3871\r\n2558\r\n1957\r\n4058\r\n1935\r\n5738\r\n1513\r\n5744\r\n4130\r\n3321\r\n3760\r\n4155\r\n1063\r\n3694\r\n\r\n67474\r\n\r\n2215\r\n8962\r\n5442\r\n7390\r\n6223\r\n3648\r\n2390\r\n5464\r\n\r\n5125\r\n3842\r\n2131\r\n3414\r\n6085\r\n3353\r\n3660\r\n1593\r\n4395\r\n2664\r\n4987\r\n2352\r\n5693\r\n1391\r\n1661\r\n\r\n6017\r\n5013\r\n5243\r\n3734\r\n3116\r\n1541\r\n6456\r\n2806\r\n6143\r\n1344\r\n5977\r\n5422\r\n2326\r\n2164\r\n\r\n33377\r\n\r\n5950\r\n5168\r\n1348\r\n3144\r\n5760\r\n6974\r\n8537\r\n6664\r\n1248\r\n\r\n9040\r\n4510\r\n5642\r\n8547\r\n9407\r\n5058\r\n6158\r\n7726\r\n6731\r\n\r\n4182\r\n5246\r\n2686\r\n4196\r\n3809\r\n3253\r\n3260\r\n3162\r\n2703\r\n6373\r\n6136\r\n2904\r\n6356\r\n5038\r\n\r\n3368\r\n11040\r\n5793\r\n13747\r\n10478\r\n5166\r\n\r\n11697\r\n10072\r\n8434\r\n5732\r\n\r\n5244\r\n6374\r\n1877\r\n2902\r\n4263\r\n4835\r\n2283\r\n2086\r\n6044\r\n6343\r\n6232\r\n2849\r\n3694\r\n6392\r\n\r\n13062\r\n12405\r\n12869\r\n16366\r\n1652\r\n\r\n8036\r\n1720\r\n7759\r\n13661\r\n1370\r\n3505\r\n\r\n5885\r\n1708\r\n10440\r\n7438\r\n11480\r\n4904\r\n8387\r\n\r\n4788\r\n10326\r\n6143\r\n11063\r\n9649\r\n9384\r\n\r\n5849\r\n1935\r\n7411\r\n4694\r\n4435\r\n1321\r\n5301\r\n2096\r\n2422\r\n5484\r\n6560\r\n3070\r\n\r\n5008\r\n5405\r\n9444\r\n6355\r\n13150\r\n12686\r\n\r\n1618\r\n5831\r\n1957\r\n1288\r\n1382\r\n2946\r\n1926\r\n1828\r\n1646\r\n4823\r\n3637\r\n1309\r\n5727\r\n4450\r\n\r\n66234\r\n\r\n4016\r\n4075\r\n4551\r\n5869\r\n2866\r\n2461\r\n1055\r\n4712\r\n7220\r\n1973\r\n4650\r\n\r\n2387\r\n1102\r\n4558\r\n2290\r\n1969\r\n3624\r\n2383\r\n4059\r\n4877\r\n3439\r\n6891\r\n3819\r\n5824\r\n\r\n3440\r\n11447\r\n5522\r\n16700\r\n\r\n8373\r\n1356\r\n7249\r\n8484\r\n6767\r\n7990\r\n8778\r\n2235\r\n5995\r\n8511\r\n\r\n5466\r\n6088\r\n2583\r\n5235\r\n6093\r\n3089\r\n1570\r\n6112\r\n3668\r\n2187\r\n5582\r\n1380\r\n4800\r\n5529\r\n2987\r\n\r\n12323\r\n4775\r\n11105\r\n3805\r\n7671\r\n6737\r\n\r\n3197\r\n1256\r\n4539\r\n6383\r\n1476\r\n2134\r\n6785\r\n7337\r\n1229\r\n4965\r\n5055\r\n\r\n13128\r\n8149\r\n4426\r\n8087\r\n2873\r\n\r\n1695\r\n8214\r\n7147\r\n7216\r\n1070\r\n2801\r\n5033\r\n8103\r\n7750\r\n5749\r\n\r\n4082\r\n1630\r\n1958\r\n6113\r\n4486\r\n2111\r\n1486\r\n6451\r\n5630\r\n5682\r\n4558\r\n\r\n10475\r\n30751\r\n\r\n5397\r\n5067\r\n1178\r\n2106\r\n5557\r\n4847\r\n1661\r\n3469\r\n4908\r\n2590\r\n4686\r\n2118\r\n4495\r\n2994\r\n\r\n12590\r\n12902\r\n8742\r\n15433\r\n2777\r\n\r\n7451\r\n8722\r\n7302\r\n2269\r\n9592\r\n6162\r\n\r\n1907\r\n6741\r\n4442\r\n6526\r\n8289\r\n7479\r\n9481\r\n1642\r\n4982\r\n\r\n2001\r\n12803\r\n7255\r\n13545\r\n10713\r\n\r\n14621\r\n25826\r\n20771\r\n\r\n1055\r\n8331\r\n7422\r\n8400\r\n2208\r\n6688\r\n8425\r\n6883\r\n4192\r\n4725\r\n\r\n9970\r\n10127\r\n2777\r\n3933\r\n9274\r\n3791\r\n9906\r\n\r\n2779\r\n1118\r\n2823\r\n1563\r\n9917\r\n6065\r\n1863\r\n1885\r\n\r\n5053\r\n1922\r\n5356\r\n8266\r\n7335\r\n4260\r\n8730\r\n7307\r\n3140\r\n\r\n4340\r\n1080\r\n2120\r\n5498\r\n6769\r\n6170\r\n2944\r\n4677\r\n1155\r\n4676\r\n2420\r\n4945\r\n5941\r\n\r\n14838\r\n3844\r\n19712\r\n16283\r\n\r\n1906\r\n3672\r\n4274\r\n6550\r\n6072\r\n5299\r\n4493\r\n3348\r\n2804\r\n3747\r\n3208\r\n5359\r\n\r\n59156\r\n\r\n6128\r\n5959\r\n12262\r\n12166\r\n4945\r\n9635\r\n\r\n8095\r\n1757\r\n13656\r\n7215\r\n2545\r\n6298\r\n\r\n7873\r\n6515\r\n6407\r\n4182\r\n4911\r\n3966\r\n1942\r\n7281\r\n5082\r\n7446\r\n5545\r\n\r\n3876\r\n5618\r\n4226\r\n4445\r\n3915\r\n1250\r\n5469\r\n3872\r\n5157\r\n5539\r\n2894\r\n1357\r\n1419\r\n4214\r\n3009\r\n\r\n3364\r\n6618\r\n1432\r\n7583\r\n6192\r\n2529\r\n2076\r\n6231\r\n7333\r\n\r\n5028\r\n2989\r\n3801\r\n1895\r\n2367\r\n5747\r\n2724\r\n1304\r\n3085\r\n2524\r\n1295\r\n1022\r\n2613\r\n4239\r\n4413\r\n\r\n3418\r\n23885\r\n\r\n3895\r\n3302\r\n1097\r\n3150\r\n1379\r\n1815\r\n1484\r\n2074\r\n2616\r\n3395\r\n6100\r\n5488\r\n1006\r\n3046\r\n\r\n1951\r\n8238\r\n6010\r\n3652\r\n6640\r\n7990\r\n3900\r\n8191\r\n\r\n6978\r\n3025\r\n1966\r\n7152\r\n5963\r\n7326\r\n1866\r\n2507\r\n4764\r\n1434\r\n3194\r\n3326\r\n\r\n3081\r\n6828\r\n6495\r\n3257\r\n4236\r\n4938\r\n7090\r\n2826\r\n6614\r\n1341\r\n7098\r\n6771\r\n\r\n18885\r\n3826\r\n13103\r\n1193\r\n\r\n16653\r\n13676\r\n19294\r\n13878\r\n\r\n7460\r\n4327\r\n7335\r\n4898\r\n6259\r\n7941\r\n5940\r\n5603\r\n3095\r\n5674\r\n2886\r\n\r\n8680\r\n4662\r\n8044\r\n8210\r\n5886\r\n9057\r\n7917\r\n3258\r\n5995\r\n\r\n2800\r\n\r\n31568\r\n\r\n5423\r\n33845\r\n\r\n8988\r\n13780\r\n9059\r\n14650\r\n9824\r\n\r\n10327\r\n13747\r\n10937\r\n4594\r\n6882\r\n\r\n2696\r\n2860\r\n1070\r\n1662\r\n6098\r\n2369\r\n2821\r\n3603\r\n3593\r\n5347\r\n4871\r\n4967\r\n2211\r\n3330\r\n5226\r\n\r\n8520\r\n1896\r\n1906\r\n5172\r\n6845\r\n1565\r\n6366\r\n5509\r\n5068\r\n\r\n4047\r\n4661\r\n2384\r\n1904\r\n4705\r\n5402\r\n6650\r\n6120\r\n4834\r\n5897\r\n6492\r\n6200\r\n5868\r\n\r\n4130\r\n1530\r\n9654\r\n9306\r\n8529\r\n2508\r\n8918\r\n4040\r\n3717\r\n\r\n3245\r\n4195\r\n11698\r\n10053\r\n6635\r\n4749\r\n\r\n12283\r\n11894\r\n8168\r\n\r\n6418\r\n7151\r\n4827\r\n2391\r\n8643\r\n2315\r\n8664\r\n3547\r\n6900\r\n4418\r\n\r\n65083\r\n\r\n5164\r\n1748\r\n8739\r\n4945\r\n2604\r\n5493\r\n5310\r\n3763\r\n6011\r\n4091\r\n\r\n4342\r\n2566\r\n6021\r\n4067\r\n1385\r\n2486\r\n4444\r\n1713\r\n3380\r\n4253\r\n4720\r\n1602\r\n4814\r\n3234\r\n1739\r\n\r\n13603\r\n22634\r\n\r\n7845\r\n\r\n4139\r\n8832\r\n7441\r\n1967\r\n6707\r\n7981\r\n3416\r\n10633\r\n\r\n4808\r\n9206\r\n6975\r\n4257\r\n6024\r\n5176\r\n\r\n10639\r\n9329\r\n5010\r\n7215\r\n4522\r\n8168\r\n9936\r\n9792\r\n\r\n4326\r\n4601\r\n6508\r\n2501\r\n3866\r\n3060\r\n2188\r\n6429\r\n1014\r\n1383\r\n6937\r\n5313\r\n4392\r\n\r\n8977\r\n2420\r\n1013\r\n4267\r\n9689\r\n2955\r\n5583\r\n2947\r\n5570\r\n\r\n5067\r\n1139\r\n4531\r\n3001\r\n7698\r\n2028\r\n8025\r\n1424\r\n6837\r\n1357\r\n2186\r\n\r\n5433\r\n19739\r\n5321\r\n\r\n5019\r\n4714\r\n2688\r\n5290\r\n1593\r\n1320\r\n1703\r\n2978\r\n1476\r\n3034\r\n3538\r\n2825\r\n5819\r\n5547\r\n\r\n6441\r\n1104\r\n3128\r\n7940\r\n5977\r\n8434\r\n4493\r\n2139\r\n1617\r\n4933\r\n\r\n3219\r\n3273\r\n12383\r\n6076\r\n11222\r\n5523\r\n\r\n8875\r\n1005\r\n4958\r\n3234\r\n3196\r\n5527\r\n9875\r\n7306\r\n\r\n1591\r\n6031\r\n2124\r\n1224\r\n7008\r\n2559\r\n4540\r\n7670\r\n\r\n5927\r\n4323\r\n5630\r\n1067\r\n5482\r\n1939\r\n5428\r\n3416\r\n3494\r\n2469\r\n4287\r\n5538\r\n1190\r\n4234\r\n1734\r\n\r\n10808\r\n15234\r\n14069\r\n10497\r\n7697");

                ctx.Status($"[yellow]Benchmarking {descriptor.Attributes.Day}/{descriptor.Attributes.Year} -- running part A {benchmarkCommandArguments.Runs} times.[/]");
                var runtimes = new List<double>();
                for (var i = 0; i < benchmarkCommandArguments.Runs; ++i)
                {
                    var then = DateTime.UtcNow;
                    _ = await descriptor.Instance.DoPartOneAsync(context);
                    runtimes.Add((DateTime.UtcNow - then).TotalMilliseconds);
                }
                AnsiConsole.MarkupLine($"Part A -- Minimum: {runtimes.Min()}ms, Maximum: {runtimes.Max()}ms, Average: {runtimes.Average()}ms.");

                ctx.Status($"[yellow]Benchmarking {descriptor.Attributes.Day}/{descriptor.Attributes.Year} -- running part B {benchmarkCommandArguments.Runs} times.[/]");
                runtimes.Clear();
                for (var i = 0; i < benchmarkCommandArguments.Runs; ++i)
                {
                    var then = DateTime.UtcNow;
                    _ = await descriptor.Instance.DoPartTwoAsync(context);
                    runtimes.Add((DateTime.UtcNow - then).TotalMilliseconds);
                }
                AnsiConsole.MarkupLine($"Part B -- Minimum: {runtimes.Min()}ms, Maximum: {runtimes.Max()}ms, Average: {runtimes.Average()}ms.");
            });
    }
}
