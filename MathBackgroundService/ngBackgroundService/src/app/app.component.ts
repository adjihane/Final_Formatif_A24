import { AccountService } from './services/account.service';
import { Component, NgZone } from '@angular/core';
import { environment } from 'src/environments/environment';

// On doit commencer par ajouter signalr dans les node_modules: npm install @microsoft/signalr
// Ensuite on inclut la librairie
import * as signalR from "@microsoft/signalr"

 enum Operation
{
    Add,
    Substract,
    Multiply
}

interface MathQuestion{
  operation:Operation;
  valueA:number;
  valueB:number;
  answers:number[];
  playerChoices:number[];
}

interface PlayerInfoDTO{
  nbRightAnswers:number;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'ngBackgroundService';

  baseUrl = environment.apiUrl;

  nbRightAnswers = 0;

  private hubConnection?: signalR.HubConnection

  isConnected = false;
  selecion = -1;

  currentQuestion: MathQuestion | null = null;

  constructor(public account:AccountService, private zone: NgZone){
  }

  SelectChoice(choice:number) {
    this.selecion = choice;
    this.hubConnection!.invoke('SelectChoice', choice)
  }

  async register(){
    try{
      await this.account.register();
    }
    catch(e){
      alert("Erreur pendant l'enregistrement!!!!!");
      return;
    }
    alert("L'enregistrement a été un succès!");
  }

  async login(){
    await this.account.login();
  }

  async logout(){
    await this.account.logout();

    if(this.hubConnection?.state == signalR.HubConnectionState.Connected)
      this.hubConnection.stop();
    this.isConnected = false;
  }

  isLoggedIn() : Boolean{
    return this.account.isLoggedIn();
  }

  connectToHub() {
    this.hubConnection = new signalR.HubConnectionBuilder()
                              .withUrl(this.baseUrl + 'game')
                              .build();

    if(!this.hubConnection)
      return;

    this.hubConnection.on('PlayerInfo', (data:PlayerInfoDTO) => {
      console.log("Received PlayerInfo");
      this.zone.run(() => {
        console.log(data);
        this.isConnected = true;
        this.nbRightAnswers = data.nbRightAnswers;
      });
    });


    this.hubConnection.on('CurrentQuestion', (data:MathQuestion) => {
      console.log("Received CurrentQuestion");
      this.zone.run(() => {
        console.log(data);
        this.selecion = -1;
        this.currentQuestion = data;
      });
    });

    this.hubConnection.on('RightAnswer', () => {
      console.log("Received RightAnswer");
      this.zone.run(() => {
        // TODO: Montrer au joueur qu'il avait la bonne réponse
        this.nbRightAnswers++;
        alert("Good Answer!");
      });
    });

    this.hubConnection.on('WrongAnswer', (rightAnswer:number) => {
      console.log("Received WrongAnswer");
      this.zone.run(() => {
        console.log(rightAnswer);
        // TODO: Montrer au joueur qu'il avait la mauvaise réponse
        alert("Wrong Answer! The right answer was: " + rightAnswer);
      });
    });

    this.hubConnection.on('IncreasePlayersChoices', (choiceIndex:number) => {
      console.log("Received IncreasePlayersChoices");
      this.zone.run(() => {
        if(this.currentQuestion){
          this.currentQuestion.playerChoices[choiceIndex]++;
        }
      });
    });

    this.hubConnection
      .start()
      .then(() => {
        console.log("Connected to Hub");
      })
      .catch(err => console.log('Error while starting connection: ' + err))
  }
}
