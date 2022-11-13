# GeneticAlgorithmTrainer
This asset is designed to train and test a neural network using the Genetic Algorithm.

## Description
I have made this asset to experiment and learn about the effectiveness of the Genetic Algorithm while using different layer structures and parameters.<br />
The example scene contains a car and a track with solid walls acting as borders, the objective is to train a neural network that tells the car where to go, so that the car can reach the end of the track without colliding with the walls.

![screenshotAICAR](https://user-images.githubusercontent.com/85197456/125550518-fb60c95e-5de3-4706-ab8a-74960a392191.png)


## Genetic Algorithm implementation in the example scene
The current neural network model consists of:
* Input layer: contains 5 nodes that represent the distance between the car and an obstacle when looking to a particular direction (Angles of direction: -45°, -22.5°, 0° (forward), 22.5°, 45°)
* Hidden layers: you can choose from multiple presets.
* Output layer: contains 2 nodes that represents the turn angle and the speed the car should get to avoid crashing.

The genotype is composed of all the weights and bias values related to the model.<br />
The first generation is created with a determined pool size, and each car is initialized with a new genotype consisting of random values.<br />
The selection method used to choose the parents of the next generation is Elitism: the cars that made the most distance are used for crossover.<br />
For the crossover method the user can choose between Uniform and One point. For each child a mutation probability is applied.<br />
Both parents and childs are carried over to the next generation. <br />

During the test of a trained neural network you can visually see the structure with a realtime graphical representation of the values.
![img2](https://user-images.githubusercontent.com/85197456/201503035-2b9ef1d0-0dc1-431b-a20c-01564b7e2feb.png)

### Parameters
* Pool size: how many cars are in each generation.
* Elitism selection size: the amount of parents that are selected for each generation.
* Mutation probability: the chance that each value in the genotype has to be mutated into a random number.
* Crossover method: method used to transfer genotype from parents to child. Currently you can choose between Uniform and One point. 
* Training course: Currently there are 3 courses available to train and test.
* NN architecture: Preset indicating the number of nodes and layers. Example: [4 4] means 2 hidden layers of 4 nodes each one.

![img1](https://user-images.githubusercontent.com/85197456/201502969-17984a3d-a06c-445f-8c1d-91d8305e3ffd.png)

### Useful links
* https://en.wikipedia.org/wiki/Genetic_algorithm
* https://en.wikipedia.org/wiki/Selection_(genetic_algorithm)
* https://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)

## Unity version
2021.3.10f1
