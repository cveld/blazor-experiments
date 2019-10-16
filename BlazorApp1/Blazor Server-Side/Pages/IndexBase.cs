using BlazorServerSide.Data;
using BlazorServerSide.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerSide.Pages
{
    public class IndexBase : ComponentBase
    {
        const string FAVORITECLICKEDEVENTID = "FavoriteClicked";

        int focusIndex = -1;
        //public IndexBase(CrossCircuitCommunication.CrossCircuitCommunication crossCircuitCommunication, VacationContext context)
        //{
        //    this.crossCircuitCommunication = crossCircuitCommunication;
        //    this.context = context;
        //}

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

        protected override async Task OnInitializedAsync()
        {
            vacations = new Dictionary<int, VacationModel>();
            foreach (var vacation in context.Vacations)
            {
                vacations.Add(vacation.ID, vacation);
                crossCircuitCommunication.GetCallbacksHashSet(FAVORITECLICKEDEVENTID, vacation.ID).Add((payload) => OnFavoriteClickedMessageHandler((FavoriteClickedModel)payload.Message, EnforceStateHasChanged: true));
    
        }
            context.Vacations.ToList();
            CurrentUser = "dummy";
        }

        bool VacationLiked(VacationModel vacation)
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

        void OnFavoriteClickedMessageHandler(FavoriteClickedModel favoriteClickedModel, bool EnforceStateHasChanged)
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
            if (EnforceStateHasChanged)
            {
                // Message is coming from external source; Blazor state needs to get a kick 
                base.InvokeAsync(StateHasChanged);
            }
        }

    }
}
