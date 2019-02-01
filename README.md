# PartyAffiliationClassifier
Artificial Intelligence assignment implementing Naive Bayes Classifier

The aim was to create a Party Affiliation Classifier that analyses the text of a Queen's Speech and predicts the political affinity of the current government at the time the speech was delivered (Conservative, Labour, Coalition).


# Algorithmic development:

1. **Determine prior probabilities** - sum up the number of documents being used for the training phase, then for each category the documents are attributed to, and last calculate the probability for each category.

**Tdocs - number of all of the documents**

**Tdocs = Tcat1 + Tcat2 + ... + TcatN**

Probabilities for the categories: 

**P(cat1) = Tcat1/Tdocs**

**P(cat2) = Tcat2/Tdocs**

**...**

**P(catN) = TcatN/Tdocs**


2. **Calculate the conditional probabilities** - now we have to sum up the number of UNIQUE words throughout the training documents (allWords), total number of words for each category including REPEATS (cat1, cat2, ..., catN). Also, for each category, count the frequency of each UNIQUE word for that category (Fcat[word]). When you have those, calculate conditional probabilities of the words in that category (P(wordI/catN)).

**P(wordI/catN) = (Fcat[word] + 1)/ (cat1 + allWords)**

**Note:** we add 1 to the frequency of the word in case there's no word like that in the category which means that the frequency would equal 0 and because of that the entire probability would be zeroed which can cause issues when classifying a unknown document later.

3. **Classify a document** - analyse a new document, and for each word compare it with the vocabulary that was created for each category with the words and their conditional probabilities. If a word in the document is also present in the category we calculate the product by multiplying the probabilities together, at the end multiplying everything by the category probability: 

**P(cat1/newDoc) = P(word1/cat1) x P(word1/cat1) X ... X P(wordI/catN)**

We have to do it for each category and the one with the highest result will be the prediction.

It's better to change the multiplication into summation of the logarithms of the conditional probabilites as the numbers will be too small and may generate floating-point errors and come out as 0.

**Also remember that if the word is present in the document but not in the category to still assign it to the final sum by using the probability - 0 + 1/(catN + allWords)**
