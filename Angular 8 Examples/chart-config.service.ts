import { Injectable } from "@angular/core";
import { Observable, of } from "rxjs";
import { map } from "rxjs/operators";
import { ChartConfig } from "../globals/chart-classes";

@Injectable({
  providedIn: "root"
})
export class ChartConfigService {
  private configs: ChartConfig[] = [];

  // Loads configs from localStorage
  private loadConfigs(): Observable<ChartConfig[]> {
    if (localStorage.length > 0) {
      let configJsonString = localStorage.getItem("chartConfigs");
      this.configs = JSON.parse(configJsonString);
    }
    return of(this.configs);
  }

  // If configs is empty, calls loadConfigs()
  getConfigs(): Observable<ChartConfig[]> {
    if (this.configs.length > 0) {
      return of(this.configs);
    } else return this.loadConfigs();
  }

  // Pushes this.configs to localStorage
  saveConfigs(): Observable<boolean> {
    localStorage.setItem("chartConfigs", JSON.stringify(this.configs));
    return of(true);
  }

  addConfig(config: ChartConfig) {
    this.configs.unshift(config);
    return this.saveConfigs();
  }

  removeConfig(config: ChartConfig) {
    let index = this.configs.indexOf(config);
    this.configs.splice(index, 1);
    this.saveConfigs();
  }

  constructor() {}
}
