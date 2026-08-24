import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import './app/core/leaflet-icon-fix';

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
