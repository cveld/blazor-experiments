using BlazorServerSide.Data;
using BlazorServerSide.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerSide.Components.VacationList
{
    public class VacationListBase : ComponentBase, IDisposable
    {
        const string FAVORITECLICKEDEVENTID = "FavoriteClicked";
       
        protected Dictionary<int, VacationModel> vacations;

        [Inject]
        public CrossCircuitCommunication.CrossCircuitCommunication crossCircuitCommunication { get; set; }

        [Inject]
        public VacationContext context { get; set; }

        [CascadingParameter]
        protected string CurrentUser { get; set; }

        protected override async Task OnParametersSetAsync()
        {
        }

        protected override void OnAfterRender(bool firstRender)
        {
        }

        List<(HashSet<Action<CrossCircuitCommunication.MessagePayload>>, Action<CrossCircuitCommunication.MessagePayload>)> subscriptions = new List<(HashSet<Action<CrossCircuitCommunication.MessagePayload>>, Action<CrossCircuitCommunication.MessagePayload>)>();

        protected override async Task OnInitializedAsync()
        {
            vacations = new Dictionary<int, VacationModel>();
            foreach (var vacation in context.Vacations)
            {
                vacations.Add(vacation.ID, vacation);         
                Action<CrossCircuitCommunication.MessagePayload> action = (payload) => OnFavoriteClickedMessageHandler((FavoriteClickedModel)payload.Message, RemoteTrigger: true);
                var hashset = crossCircuitCommunication.GetCallbacksHashSet(FAVORITECLICKEDEVENTID, vacation.ID);
                hashset.Add(action);
                subscriptions.Add((hashset, action));
            }            
            CurrentUser = "dummy";
        }

        protected bool VacationLiked(VacationModel vacation)
        {
            var user = vacation.Likes?.Where(u => u.Name == CurrentUser).FirstOrDefault();
            return user != null;
        }

        async protected void OnFavoriteClicked(VacationModel vacationModel)
        {
            var message = new FavoriteClickedModel
            {
                VacationId = vacationModel.ID,
                Liked = !VacationLiked(vacationModel)
            };
            
            OnFavoriteClickedMessageHandler(message, false);
            await crossCircuitCommunication.Dispatch(FAVORITECLICKEDEVENTID, vacationModel.ID, message);
        }

        void OnFavoriteClickedMessageHandler(FavoriteClickedModel favoriteClickedModel, bool RemoteTrigger)
        {
            var vacation = vacations[favoriteClickedModel.VacationId];
            var currentstate = VacationLiked(vacation);
            var desiredstate = favoriteClickedModel.Liked;
            if (currentstate != desiredstate)
            {
                var user = vacation.Likes?.Where(u => u.Name == CurrentUser).FirstOrDefault();

                if (!desiredstate)
                {
                    vacation.Likes.Remove(user);
                }
                else
                {
                    if (vacation.Likes == null)
                    {
                        vacation.Likes = new HashSet<User>();
                    }
                    vacation.Likes.Add(new User
                    {
                        Name = CurrentUser
                    });
                }
            }
            if (RemoteTrigger)
            {
                // Message is coming from external source; Blazor state needs to get a kick 
                base.InvokeAsync(StateHasChanged);
            }
            else
            {
                // Method is directly called due to the user's action, persist the change:
                context.SaveChanges();
            }
        }

        public void Dispose()
        {
            foreach (var item in subscriptions)
            {
                // Item1 = HashSet
                // Item2 = anonymous Action 
                item.Item1.Remove(item.Item2);
            }
        }
    }
}
