import { transition, trigger, useAnimation } from '@angular/animations';
import { Component } from '@angular/core';
import { bounce, shake, shakeX, tada } from 'ng-animate';
import { lastValueFrom, timer } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  animations: [
    trigger('shake', [transition(':increment', useAnimation(shake, { params: { timing: 2 } }))]),
    trigger('bounce', [transition(':increment', useAnimation(bounce, { params: { timing: 4 } }))]),
    trigger('tada', [transition(':increment', useAnimation(tada, { params: { timing: 3 } }))]),
  ],
})
export class AppComponent {
  title = 'ngAnimations';

  css_shake = 0;
  css_bounce = 0;
  css_tada = 0;

  private _enboucle = false;
  rotate = false;

  constructor() {
  }

  rotateMe() {
    this.rotate = true;
    setTimeout(() => { this.rotate = false; }, 2000);
  }

  async laSequence(boucle: boolean) {
    this.css_shake++;

    await this.waitFor(2);
    this.css_bounce++;

    await this.waitFor(3);
    this.css_tada++;

    await this.waitFor(3);
    if (boucle && this._enboucle) {
      this.enBoucle();
    }
  }

  async waitFor(delayInSeconds: number) {
    await lastValueFrom(timer(delayInSeconds * 1000));
  }

  enSequence() {
    this._enboucle = false;
    this.laSequence(false)
  }
  async enBoucle() {
    this._enboucle = true;
    this.laSequence(true)
  }
}
