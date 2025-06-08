//
//  ContentView.swift
//  ona
//
//  Created by Natalia Naumova on 29/07/2024.
//

import SwiftUI

struct ContentView: View {
    @State private var startString: String
    @State private var durationString: String
    @State private var intervalString: String
    
    var body: some View {
        return VStack {
            Image(systemName: "globe")
                .imageScale(.large)
                .foregroundStyle(.tint)
            HStack {
                Text("Start:")
                TextField("Start", text: $startString)
            }
            HStack {
                Text("Duration:")
                TextField("Duration", text: $durationString)
            }
            HStack {
                Text("Interval:")
                TextField("Interval", text: $intervalString)
            }
            Button("Save") {
                let periodState = PeriodState(startDate: startString, duration: Int(durationString)!, interval: Int(intervalString)!)
                let periodStateData = try! JSONEncoder().encode(periodState)
                let periodStateString = String(data: periodStateData, encoding: .utf8)
                UserDefaults(suiteName: "group.com.natalianaumova.ona")!.set(periodStateString, forKey: "PeriodState")
            }
        }
        .padding()
    }
    
    init(periodState: PeriodState = getPeriodState()) {
        self.startString = periodState.startDate
        self.durationString = "\(periodState.duration)"
        self.intervalString = "\(periodState.interval)"
    }
    
    static func getPeriodState() -> PeriodState {
        let fallback = PeriodState(startDate: "2001-01-01", duration: 0, interval: 0)
        
        guard let userDefault = UserDefaults(suiteName: "group.com.natalianaumova.ona")?.string(forKey: "PeriodState") else {
            return fallback
        }
        let data = Data(userDefault.utf8)
        let periodState = try? JSONDecoder().decode(PeriodState.self, from: data)
        return periodState ?? fallback
    }
}

struct PeriodState : Codable {
    var startDate: String
    var duration: Int
    var interval: Int
}

#Preview {
    ContentView()
}
