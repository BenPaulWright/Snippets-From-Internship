import { Injectable } from "@angular/core";
import { Observable, of } from "rxjs";
import { ChartConfig } from "../globals/chart-classes";

@Injectable({
  providedIn: "root"
})
export class ChartDataService {
  private data: Observable<any[]>;

  // Do more filtering and get the right data from the server
  private loadData(config: ChartConfig): Observable<any[]> {
    if (config.type == "VerticalBar") {
      return of(this.getDefaultVerticalBarData());
    }
    if (config.type == "Line") {
      return of(this.getDefaultLineData());
    }
    if (config.type == "Pie") {
      return of(this.getDefaultPieData());
    }
    if (config.type == "GroupedVerticalBar") {
      return of(this.getDefaultGroupedVerticalBarData());
    }
  }

  // Do more filtering and get the right data from the server
  getData(config: ChartConfig): Observable<any[]> {
    if (typeof this.data !== "undefined") {
      return this.data;
    } else return this.loadData(config);
  }

  // Do more filtering and get the right data from the server
  getColorScheme(config: ChartConfig): Observable<any> {
    if (config.type == "VerticalBar") {
      return of(this.defaultVerticalBarColorScheme());
    }
    if (config.type == "Line") {
      return of(this.getDefaultLineColorScheme());
    }
    if (config.type == "Pie") {
      return of(this.getDefaultPieColorScheme());
    }
    if (config.type == "GroupedVerticalBar") {
      return of(this.getDefaultGroupedVerticalBarColorScheme());
    }
  }

  constructor() {}

  private randZeroToTen(): number {
    return Math.floor(Math.random() * 10);
  }

  // Random Default Values
  getDefaultPieColorScheme(): any {
    return {
      domain: [
        "#5AA454",
        "#E44D25",
        "#CFC0BB",
        "#7aa3e5",
        "#7aa3e5",
        "#7aa3e5",
        "#7aa3e5",
        "#7aa3e5"
      ]
    };
  }

  getDefaultPieData(): DataPoint[] {
    return [
      { name: "0", value: this.randZeroToTen() },
      { name: "1", value: this.randZeroToTen() },
      { name: "2", value: this.randZeroToTen() },
      { name: "3", value: this.randZeroToTen() },
      { name: "4", value: this.randZeroToTen() },
      { name: "5", value: this.randZeroToTen() }
    ];
  }

  defaultVerticalBarColorScheme(): any {
    return {
      domain: [
        "#5AA454",
        "#E44D25",
        "#CFC0BB",
        "#7aa3e5",
        "#7aa3e5",
        "#7aa3e5",
        "#7aa3e5",
        "#7aa3e5"
      ]
    };
  }

  getDefaultVerticalBarData(): DataPoint[] {
    return [
      { name: "0", value: this.randZeroToTen() },
      { name: "1", value: this.randZeroToTen() },
      { name: "2", value: this.randZeroToTen() },
      { name: "3", value: this.randZeroToTen() },
      { name: "4", value: this.randZeroToTen() },
      { name: "5", value: this.randZeroToTen() }
    ];
  }

  getDefaultLineColorScheme(): any {
    return {
      domain: [
        "#5AA454",
        "#E44D25",
        "#CFC0BB",
        "#7aa3e5",
        "#7aa3e5",
        "#7aa3e5",
        "#7aa3e5",
        "#7aa3e5"
      ]
    };
  }

  getDefaultLineData(): SingleDataSet[] {
    return [
      {
        name: "Default Dataset 1",
        series: [
          { name: "0", value: this.randZeroToTen() },
          { name: "1", value: this.randZeroToTen() },
          { name: "2", value: this.randZeroToTen() },
          { name: "3", value: this.randZeroToTen() },
          { name: "4", value: this.randZeroToTen() },
          { name: "5", value: this.randZeroToTen() },
          { name: "6", value: this.randZeroToTen() },
          { name: "7", value: this.randZeroToTen() },
          { name: "8", value: this.randZeroToTen() },
          { name: "9", value: this.randZeroToTen() },
          { name: "10", value: this.randZeroToTen() }
        ]
      },
      {
        name: "Default Dataset 2",
        series: [
          { name: "0", value: this.randZeroToTen() },
          { name: "1", value: this.randZeroToTen() },
          { name: "2", value: this.randZeroToTen() },
          { name: "3", value: this.randZeroToTen() },
          { name: "4", value: this.randZeroToTen() },
          { name: "5", value: this.randZeroToTen() },
          { name: "6", value: this.randZeroToTen() },
          { name: "7", value: this.randZeroToTen() },
          { name: "8", value: this.randZeroToTen() },
          { name: "9", value: this.randZeroToTen() },
          { name: "10", value: this.randZeroToTen() }
        ]
      },
      {
        name: "Default Dataset 3",
        series: [
          { name: "0", value: this.randZeroToTen() },
          { name: "1", value: this.randZeroToTen() },
          { name: "2", value: this.randZeroToTen() },
          { name: "3", value: this.randZeroToTen() },
          { name: "4", value: this.randZeroToTen() },
          { name: "5", value: this.randZeroToTen() },
          { name: "6", value: this.randZeroToTen() },
          { name: "7", value: this.randZeroToTen() },
          { name: "8", value: this.randZeroToTen() },
          { name: "9", value: this.randZeroToTen() },
          { name: "10", value: this.randZeroToTen() }
        ]
      },
      {
        name: "Default Dataset 4",
        series: [
          { name: "0", value: this.randZeroToTen() },
          { name: "1", value: this.randZeroToTen() },
          { name: "2", value: this.randZeroToTen() },
          { name: "3", value: this.randZeroToTen() },
          { name: "4", value: this.randZeroToTen() },
          { name: "5", value: this.randZeroToTen() },
          { name: "6", value: this.randZeroToTen() },
          { name: "7", value: this.randZeroToTen() },
          { name: "8", value: this.randZeroToTen() },
          { name: "9", value: this.randZeroToTen() },
          { name: "10", value: this.randZeroToTen() }
        ]
      },
      {
        name: "Default Dataset 5",
        series: [
          { name: "0", value: this.randZeroToTen() },
          { name: "1", value: this.randZeroToTen() },
          { name: "2", value: this.randZeroToTen() },
          { name: "3", value: this.randZeroToTen() },
          { name: "4", value: this.randZeroToTen() },
          { name: "5", value: this.randZeroToTen() },
          { name: "6", value: this.randZeroToTen() },
          { name: "7", value: this.randZeroToTen() },
          { name: "8", value: this.randZeroToTen() },
          { name: "9", value: this.randZeroToTen() },
          { name: "10", value: this.randZeroToTen() }
        ]
      },
      {
        name: "Default Dataset 6",
        series: [
          { name: "0", value: this.randZeroToTen() },
          { name: "1", value: this.randZeroToTen() },
          { name: "2", value: this.randZeroToTen() },
          { name: "3", value: this.randZeroToTen() },
          { name: "4", value: this.randZeroToTen() },
          { name: "5", value: this.randZeroToTen() },
          { name: "6", value: this.randZeroToTen() },
          { name: "7", value: this.randZeroToTen() },
          { name: "8", value: this.randZeroToTen() },
          { name: "9", value: this.randZeroToTen() },
          { name: "10", value: this.randZeroToTen() }
        ]
      }
    ];
  }

  getDefaultGroupedVerticalBarColorScheme(): any {
    return {
      domain: ["#5AA454", "#E44D25", "#CFC0BB"]
    };
  }

  getDefaultGroupedVerticalBarData(): SingleDataSet[] {
    return [
      {
        name: "Default Dataset 1",
        series: [
          { name: "0", value: this.randZeroToTen() },
          { name: "1", value: this.randZeroToTen() },
          { name: "2", value: this.randZeroToTen() },
          { name: "3", value: this.randZeroToTen() },
          { name: "4", value: this.randZeroToTen() },
          { name: "5", value: this.randZeroToTen() },
          { name: "6", value: this.randZeroToTen() },
          { name: "7", value: this.randZeroToTen() },
          { name: "8", value: this.randZeroToTen() },
          { name: "9", value: this.randZeroToTen() },
          { name: "10", value: this.randZeroToTen() }
        ]
      },
      {
        name: "Default Dataset 2",
        series: [
          { name: "0", value: this.randZeroToTen() },
          { name: "1", value: this.randZeroToTen() },
          { name: "2", value: this.randZeroToTen() },
          { name: "3", value: this.randZeroToTen() },
          { name: "4", value: this.randZeroToTen() },
          { name: "5", value: this.randZeroToTen() },
          { name: "6", value: this.randZeroToTen() },
          { name: "7", value: this.randZeroToTen() },
          { name: "8", value: this.randZeroToTen() },
          { name: "9", value: this.randZeroToTen() },
          { name: "10", value: this.randZeroToTen() }
        ]
      },
      {
        name: "Default Dataset 3",
        series: [
          { name: "0", value: this.randZeroToTen() },
          { name: "1", value: this.randZeroToTen() },
          { name: "2", value: this.randZeroToTen() },
          { name: "3", value: this.randZeroToTen() },
          { name: "4", value: this.randZeroToTen() },
          { name: "5", value: this.randZeroToTen() },
          { name: "6", value: this.randZeroToTen() },
          { name: "7", value: this.randZeroToTen() },
          { name: "8", value: this.randZeroToTen() },
          { name: "9", value: this.randZeroToTen() },
          { name: "10", value: this.randZeroToTen() }
        ]
      }
    ];
  }
}

export interface SingleDataSet {
  name: string;
  series: DataPoint[];
}

export interface DataPoint {
  name: string;
  value: number;
}
