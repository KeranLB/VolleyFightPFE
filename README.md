# VolleyFightPFE

## Intentions
L'expérience cible est un match 2 contre 2 avec une intensité croissante.

## Comment jouer
Deux équipes s'affrontent, les bleus à gauche contre les jaunes.
Il faut attaquer la balle en faisant une touche attaque et en se plaçant face à la balle pour la toucher avec la zone rectangulaire (rouge=inactive, bleue=active).
Si un joueur touche la balle, il a quelques secondes pour orienter son tir avant que la balle ne reparte automatiquement.
La balle prend la couleur de la derniere équipe qui l'a touchée.
Les équipes doivent donc s'envoyer la balle en essayant de se toucher directement. Si la balle touche un joueur, elle rebondit sur place et prend sa couleur après lui avoir ingligé des dégâts.
La zone de block (sphere autour du joueur, rouge = inactive, verte = active) permet de faire rebondir la balle sans perdre de PV.
Une barre de vie est présente au dessus de chaque joueur.
Un joueur n'ayant plus de vie est éliminé et se fait téléporter hors de l'arène.
Une fois tous les joueurs d'une équipe éliminés, l'équipe peut considérer être perdante.

Appuyer sur F10 pour réinitialiser la position de la balle (fin de round ou si elle sort du terrain par exemple).
Appuyer sur F11 pour réintialiser la position et la vie des joueurs (fin de round par exemple).

## Lancement du prototype
Dans la scène "TestBall", lancer la scène pour avoir un 2v2 avec 1 clavier et 3 manettes.
Pour changer les contrôles : sur les objets "PlayerX-Y" (X = l'équipe, Y = le numéro du joueur dans l'équipe), repérer les scripts "HumanPlayerInput" et "BotPlayerInput".
Le script "HumanPlayerInput" peut être activé et paramétré pour réagir au clavier (décocher IsGamepad) ou bien à une manette (cocher IsGamepad et indiquer un index, sachant que 0 = la première manette, 1 = la deuxième...).
Le script "BotPlayerInput" peut être activé pour faire jouer un bot automatiquement.
Ne pas activer en même temps les scripts "HumanPlayerInput" et "BotPlayerInput" sur le même joueur.

Pour jouer à moins de 4 joueurs, on peut juste désactiver les objets "PlayerX-Y" en trop.

## Contrôles

| Action        | Input (keyboard) | Input (controller)   | Input 2 (controller) |
|---------------|------------------|----------------------|----------------------|
| Move          | Z/Q/S/D          | Left Joystick        | /                    |
| Look around   | Move mouse       | Right Joystick       | /                    |
| (double) Jump | Space            | South Button         | Left Trigger         |
| Float         | Hold Space       | Hold South Button    | Hold Left Trigger    |
| Attack 1      | Left click       | West Button          | Right Shoulder       |
| Attack 2      | Right click      | North Button         | Right Trigger        |
| Block         | E                | East Button          | Left Shoulder        |
| Aim on hit    | Z/Q/S/D          | Left Joystick        | /                    |
| Lock camera   | Tab              | Right joystick click | /                    |
| Reset ball    | F10              | /                    | /                    |
| Reset players | F11              | /                    | /                    |

## Scène Blockout
Pour observer le blockout d'intention avec les lumières, lancer la scène "Blockout"
