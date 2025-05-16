using BackgroundServiceVote.Data;
using BackgroundServiceVote.Hubs;
using BackgroundServiceVote.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;

namespace BackgroundServiceVote.Services
{
    public class UserData
    {
        public int Choice { get; set; } = -1;
        public int NbConnections { get; set; } = 0;
    }

    public class MathBackgroundService : BackgroundService
    {
        public const int DELAY = 20 * 1000;

        private Dictionary<string, UserData> _data = new();

        private IHubContext<MathQuestionsHub> _mathQuestionHub;

        private MathQuestion? _currentQuestion;

        public MathQuestion? CurrentQuestion => _currentQuestion;

        private MathQuestionsService _mathQuestionsService;

        private IServiceScopeFactory _serviceScopeFactory;

        public MathBackgroundService(IHubContext<MathQuestionsHub> mathQuestionHub, MathQuestionsService mathQuestionsService, IServiceScopeFactory serviceScopeFactory)
        {
            _mathQuestionHub = mathQuestionHub;
            _mathQuestionsService = mathQuestionsService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void AddUser(string userId)
        {
            if (!_data.ContainsKey(userId))
            {
                _data[userId] = new UserData();
            }
            _data[userId].NbConnections++;
        }

        public void RemoveUser(string userId)
        {
            if (!_data.ContainsKey(userId))
            {
                _data[userId].NbConnections--;
                if (_data[userId].NbConnections <= 0)
                    _data.Remove(userId);
            }
        }

        public async void SelectChoice(string userId, int choice)
        {
            if (_currentQuestion == null)
                return;

            UserData userData = _data[userId];

            if (userData.Choice != -1)
                throw new Exception("A user cannot change is choice!");

            userData.Choice = choice;

            _currentQuestion.PlayerChoices[choice]++;

            // TODO: Notifier les clients qu'un joueur a choisi une réponse
            await _mathQuestionHub.Clients.All.SendAsync("IncreasePlayersChoices", userId, choice);
        }

        private async Task EvaluateChoices()
        {
            // TODO: La méthode va avoir besoin d'un scope
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                BackgroundServiceContext dbContext = scope.ServiceProvider.GetRequiredService<BackgroundServiceContext>();

                foreach (var userId in _data.Keys)
                {
                    var userData = _data[userId];
                    // TODO: Notifier les clients pour les bonnes et mauvaises réponses
                    // TODO: Modifier et sauvegarder le NbRightAnswers des joueurs qui ont la bonne réponse
                    if (userData.Choice == _currentQuestion!.RightAnswerIndex)
                    {
                        Player? player = await dbContext.Player.SingleOrDefaultAsync(p => p.UserId == userId);
                        player.NbRightAnswers++;
                        await dbContext.SaveChangesAsync();
                        await _mathQuestionHub.Clients.All.SendAsync("GoodAnswer", userId, player, "Bonne réponse!");

                    }
                    else
                    {
                        await _mathQuestionHub.Clients.All.SendAsync("BadAnswer", "Mauvaise réponse! La bonne réponse était " + CurrentQuestion.Answers[CurrentQuestion.RightAnswerIndex]);
                    }

                }
                // Reset
                foreach (var key in _data.Keys)
                {
                    _data[key].Choice = -1;
                }
                await dbContext.SaveChangesAsync();
            }
        }

        private async Task Update(CancellationToken stoppingToken)
        {
            if (_currentQuestion != null)
            {
                await EvaluateChoices();
            }

            _currentQuestion = _mathQuestionsService.CreateQuestion();

            await _mathQuestionHub.Clients.All.SendAsync("CurrentQuestion", _currentQuestion);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Update(stoppingToken);
                await Task.Delay(DELAY, stoppingToken);
            }
        }
    }
}
