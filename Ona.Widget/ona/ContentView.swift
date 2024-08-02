//
//  ContentView.swift
//  ona
//
//  Created by Natalia Naumova on 29/07/2024.
//

import SwiftUI

struct ContentView: View {
    @State private var counter = UserDefaults(suiteName: "group.com.natalianaumova.ona")!.integer(forKey: "Counter");
    var body: some View {
        VStack {
            Image(systemName: "globe")
                .imageScale(.large)
                .foregroundStyle(.tint)
            Text("Count: \(counter)")
            Button("Increment") {
                counter += 1;
                UserDefaults(suiteName: "group.com.natalianaumova.ona")!.set(counter, forKey: "Counter");
            }
        }
        .padding()
    }
}

#Preview {
    ContentView()
}
