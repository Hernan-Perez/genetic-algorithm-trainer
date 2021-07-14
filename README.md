# AI.GeneticAlgorithm.Test
I have made this project to experiment with the Genetic Algorithm which I find fascinating.

## Description
This is made to experiment and learn the effectiveness of the Genetic Algorithm while using different parameters.<br />
The test contains a car and a track with solid walls acting as borders, the objective is to train a neural network that tells the car when to turn and the magnitude of the angle, so that the car can reach the end of the track without colliding with the walls.

![screenshotAICAR](https://user-images.githubusercontent.com/85197456/125550518-fb60c95e-5de3-4706-ab8a-74960a392191.png)


## Genetic Algorithm implementation
The current neural network model consists of:
* Input layer: contains 5 nodes that represent the distance between the car and an obstacle when looking to a particular direction (Angles of direction: -45°, -22.5°, 0° (forward), 22.5°, 45°)
* Hidden layer 1: contains 4 nodes
* Hidden layer 2: contains 4 nodes
* Output layer: contains 2 nodes that represents the turn angle and the speed the car should get to avoid crashing.
<br /><br />
The genotype is composed of all the weights and bias values related to the model.<br />
The first generation is created with a determined pool size, and each car is initialized with a new genotype consisting of random values.<br />
The selection method used to choose the parents of the next generation is Elitism: the cars that made the most distance are used for crossover.<br />
For the crossover method the user can choose between Uniform and One point. For each child a mutation probability is applied.<br />
Both parents and childs are carried over to the next generation. <br />

### Parameters
* Pool size: how many cars are in each generation.
* Mutation probability: the chance that each value in the genotype has to be mutated into a random number.
* Elitism selection size: the amount of parents that are selected for each generation.
* Crossover method: method used to transfer genotype from parents to child. Can choose between Uniform and One point. 

### Useful links
* https://en.wikipedia.org/wiki/Genetic_algorithm
* https://en.wikipedia.org/wiki/Selection_(genetic_algorithm)
* https://en.wikipedia.org/wiki/Crossover_(genetic_algorithm)

## Unity version
2021.1.7f1

## Future of the project
There are some hardcoded elements that could be parameterized in a future to test the different results when changing their values. For example: the amount of hidden layers and the amount of nodes inside them, or adding different selection methods. <br/>
Also at some point I should add different tracks to test the behaviour of a trained NN on a new track and add a functionality allowing to save and load a trained NN.
