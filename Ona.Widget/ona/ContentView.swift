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
                let dateFormatter = DateFormatter()
                dateFormatter.dateFormat = "yyyy'-'MM'-'dd"
                
                let periodState = PeriodState(start: dateFormatter.date(from: startString)!, duration: Int(durationString)!, interval: Int(intervalString)!)
                let encodedPeriodState = try! JSONEncoder().encode(periodState)
                UserDefaults(suiteName: "group.com.natalianaumova.ona")!.set(encodedPeriodState, forKey: "PeriodState")
            }
        }
        .padding()
    }
    
    init(periodState: PeriodState = getPeriodState()) {
        let dateFormatter = DateFormatter()
        dateFormatter.dateFormat = "yyyy'-'MM'-'dd"
        
        self.startString = dateFormatter.string(from: periodState.start)
        self.durationString = "\(periodState.duration)"
        self.intervalString = "\(periodState.interval)"
    }
    
    static func getPeriodState() -> PeriodState {
        let fallback = PeriodState(start: Date(), duration: 0, interval: 0)
        
        guard let userDefault = UserDefaults(suiteName: "group.com.natalianaumova.ona")?.object(forKey: "PeriodState") else {
            return fallback
        }
        let periodState = try? JSONDecoder().decode(PeriodState.self, from: userDefault as! Data)
        return periodState ?? fallback
    }
}

struct PeriodState : Codable {
    var start: Date
    var duration: Int
    var interval: Int
}

#Preview {
    ContentView()
}
