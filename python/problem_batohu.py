import numpy as np
import pandas as pd
import random as rd
import copy

with open('input_data_1000.txt') as f:
    lines = f.readlines()

item_weights = []
item_values = []
first_line = lines[0].split()
backpack_capacity = int(first_line[1])
n = int(first_line[0])
lines.pop(0)


for line0 in lines:
    line = line0.split()
    item_weights.append(int(line[1]))
    item_values.append(int(line[0]))

#array populace bude mít stejný počet prvků jako je počet předmětů
#které můžu do batohu vložit. jedince, které chci do batohu vložit, budou mít
#na svém indexu v populaci 1, ty, které tam nechci dát 0.
 
solutions_per_pop = 12000
initial_population = []
    
for i in range(0,solutions_per_pop):
    individual = np.random.choice([0, 1], size=(n,), p=[1/2, 1/2])
    initial_population.append(individual)

#fitness udává suma cen předmětů, které jsou v batohu, tedy v jsou v populaci
def cal_fitness(item_weight, item_value, population, backpack_capacity):
    fitness = []
    for i in range(0,len(population)):
        S1 = np.sum(population[i] * item_value)
        S2 = np.sum(population[i] * item_weight)
        if S2 <= backpack_capacity:
            fitness.append(S1)
        else :
            fitness.append(-(S2-backpack_capacity)) 
    return fitness

#turnajová selekce
def selection(population,fitness_value, k): 
    new_population = []
    for i in range(0,len(population)):
        individuals = []
        fitnesses = []
        for _ in range(0,k):
            idx = rd.randint(0,len(population)-1)
            individuals.append(population[idx])
            fitnesses.append(fitness_value[idx])
        new_population.append(copy.deepcopy(individuals[np.argmax(fitnesses)]))
    return new_population

#čas na jednobodové křížení, náhodně zvolím bod v jedinci a hodnoty před 
#tímto bodem vezmeme z jednoho rodiče a hodnoty po tomto bodu z druhého
def crossover(population, cross_prob=1):
    new_population = []
    
    for i in range(0,len(population)//2):
        indiv1 = copy.deepcopy(population[2*i])
        indiv2 = copy.deepcopy(population[2*i+1])
            
        if rd.random()<cross_prob:
            # zvolime index krizeni nahodne
            crossover_point = rd.randint(0, len(indiv1)) 
            end2 =  copy.deepcopy(indiv2[:crossover_point])
            indiv2[:crossover_point] = indiv1[:crossover_point]
            indiv1[:crossover_point] = end2
            
        new_population.append(indiv1)
        new_population.append(indiv2)
        
    return new_population

#volím náhodně 'chromozomy', které budou mutovat
#mutování: pokud zvolený 'gen' je 1, po mutaci bude 0 a obráceně
def mutation(population, indiv_mutation_prob=0.1,bit_mutation_prob=0.2): 
    new_population = []
    
    for i in range(0,len(population)):
        individual = copy.deepcopy(population[i])
        if rd.random() < indiv_mutation_prob:
            for j in range(0,len(individual)):
                if rd.random() < bit_mutation_prob:
                    if individual[j]==1:
                        individual[j] = 0
                    else:
                        individual[j] = 1                        
        new_population.append(individual)
        
    return new_population

def evolution(population,  max_generations, item_weights, item_values, backpack_capacity):
    max_fitness = []
    for i in range(0,max_generations):
        fitness = cal_fitness(item_weights, item_values, population, backpack_capacity)
        max_fitness.append(max(fitness))
        parents = selection(population, fitness, 2)
        children = crossover(parents)
        mutated_children = mutation(children)
        population = mutated_children
        
    # spocitame fitness i pro posledni populaci
    fitness = cal_fitness(item_weights, item_values, population, backpack_capacity)
    max_fitness.append(max(fitness))
    best_individual = population[np.argmax(fitness)]
    
    return best_individual, population, max_fitness

best, population, max_fitness = evolution(initial_population, 150, item_weights, item_values, backpack_capacity)
print(sum(best*item_values))
